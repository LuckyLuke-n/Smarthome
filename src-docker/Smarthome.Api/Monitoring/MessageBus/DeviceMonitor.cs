using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Metrics.Abstractions;
using Smarthome.Api.Diagnostics.Meters;
using Smarthome.Api.Monitoring.MessageBus.Helpers;
using Smarthome.Api.Repositories.Devices;
using Smarthome.Core.DomainObjects;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace Smarthome.Api.Monitoring.MessageBus
{
	public class DeviceMonitor : IHostedService
	{
		private IConnectionHandler ConnectionHandler { get; }
		private IDeviceRepository DeviceRepository { get; }
		private IMetricsLogger<EnvironmentSensorData> SensorDataLogger { get; }
		private ILogger<DeviceMonitor> Logger { get; }

		private BufferBlock<PayloadContainer> MetricsBuffer { get; } = new();
		private ConcurrentDictionary<string, DeviceCacheItem> DevicesCache { get; } = [];

		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private Timer DevicesTimer { get; }
		private ConcurrentDictionary<string, ISubscriber> Sources { get; } = [];

		public DeviceMonitor( IConnectionHandler connectionHandler,
			IDeviceRepository deviceRepository,
			IMetricsLogger<EnvironmentSensorData> payloadLogger,
			ILogger<DeviceMonitor> logger )
		{
			ConnectionHandler = connectionHandler;
			DeviceRepository = deviceRepository;
			SensorDataLogger = payloadLogger;
			Logger = logger;
			DevicesTimer = new( CrawlDeviceRepositoryAsync, null, int.MaxValue, int.MaxValue );
		}

		public async Task StartAsync( CancellationToken cancellationToken )
		{
			DevicesTimer.Change( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 30 ) );
			await Task.Run( WorkOnMetricsBufferAsync, cancellationToken ).ConfigureAwait( false );
		}

		private async void WorkOnMetricsBufferAsync()
		{
			EnvironmentMetrics metrics = new();

			while ( !CancellationTokenSource.IsCancellationRequested )
			{
				try
				{				
					var container = await MetricsBuffer.ReceiveAsync( CancellationTokenSource.Token ).ConfigureAwait( false );
					if ( DevicesCache.TryGetValue( container.Topic, out var cacheItem ) && !cacheItem.IsExpired )
					{
						EnvironmentSensorData data = new( container.Payload, cacheItem.Value );
						SensorDataLogger.SendBuffered( data );
					}
					else
					{
						var response = await DeviceRepository.ReadAsync( Device.DateSourceToHostname( container.Topic ), Device.DataSourceToHardwareType( container.Topic ), CancellationTokenSource.Token );

						if ( !response.IsSuccess )
						{
							Logger.LogWarning( "Cannot retrieve device with {Topic} from repository.", container.Topic );
							continue;
						}

						DeviceCacheItem newItem = new( response.ValueSuccess! );
						DevicesCache.AddOrUpdate( container.Topic, newItem, ( key, value ) => newItem );
						EnvironmentSensorData data = new( container.Payload, newItem.Value );
						//SensorDataLogger.SendBuffered( data );

						metrics.Update( data.Temperature, data.Humidity, newItem.Value.Location, newItem.Value.Hardware.Model );
					}
				}
				catch ( Exception ex )
				{
					Logger.LogInformation( ex, "MetricsBuffer was cancelled." );
				}
			}
		}

		private async void CrawlDeviceRepositoryAsync( object? state )
		{
			if ( CancellationTokenSource.IsCancellationRequested )
				return;

			var response = await DeviceRepository.ReadReadyAndSendingAsync( CancellationTokenSource.Token ).ConfigureAwait( false );

			if ( !response.IsSuccess )
			{
				Logger.LogWarning( "Devices could not be read from MongoDb. Original error: {Error} with status {StatusCode}", response.ValueFail.Message, response.ValueFail.StatusCode );
				return;
			}

			var devices = response.ValueSuccess!;

			foreach ( var datasource in devices.Select( d => d.DataSource ) )
			{
				if ( !Sources.ContainsKey( datasource ) )
				{
					var subscriber = await ConnectionHandler.GetSubscriber( datasource, CancellationTokenSource.Token ).ConfigureAwait( false );
					subscriber.RegisterCallback( Received );

					Sources.TryAdd( datasource, subscriber );
				}
			}

			var unsubscribe = Sources.Where( s => !devices.Select( d => d.DataSource ).Contains( s.Key ) );

			foreach ( var device in unsubscribe )
				ConnectionHandler.DisconnectSubscriber( device.Value );
		}

		private void Received( string topic, string data )
		{
			try
			{
				var payload = JsonSerializer.Deserialize<Payload>( data );

				if ( payload is null )
				{
					Logger.LogWarning( "Deserializing payload for {Topic} returned null.", topic );
					return;
				}

				PayloadContainer container = new()
				{
					Topic = topic,
					Payload = payload,
				};

				MetricsBuffer.Post( container );
			}
			catch ( JsonException ex )
			{
				Logger.LogWarning( ex, "Could not deserialize for {Topic}", topic );
			}
		}

		public async Task StopAsync( CancellationToken cancellationToken )
		{
			await CancellationTokenSource.CancelAsync().ConfigureAwait( false );
			await DevicesTimer.DisposeAsync().ConfigureAwait( false );
			ConnectionHandler.Dispose();
		}
	}
}
