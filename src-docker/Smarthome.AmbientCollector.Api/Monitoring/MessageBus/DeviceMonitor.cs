using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using LSoftware.Communication.Abstractions.MessageBus;
using Smarthome.AmbientCollector.Api.Diagnostics.Meters;
using Smarthome.AmbientCollector.Api.Monitoring.MessageBus.Helpers;
using Smarthome.AmbientCollector.Api.Repositories.Devices;
using Smarthome.Core.DomainObjects;

namespace Smarthome.AmbientCollector.Api.Monitoring.MessageBus
{
	public class DeviceMonitor : IHostedService
	{
		private IConnectionHandler ConnectionHandler { get; }
		private IDeviceRepository DeviceRepository { get; }
		private ILogger<DeviceMonitor> Logger { get; }

		private BufferBlock<PayloadContainer> MetricsBuffer { get; } = new();
		private ConcurrentDictionary<string, DeviceCacheItem> DevicesCache { get; } = [];

		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private Timer DevicesTimer { get; }
		private ConcurrentDictionary<string, ISubscriber> Sources { get; } = [];

		public DeviceMonitor( IConnectionHandler connectionHandler,
			IDeviceRepository deviceRepository,
			ILogger<DeviceMonitor> logger )
		{
			ConnectionHandler = connectionHandler;
			DeviceRepository = deviceRepository;
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
			while ( !CancellationTokenSource.IsCancellationRequested )
			{
				try
				{				
					var container = await MetricsBuffer.ReceiveAsync( CancellationTokenSource.Token ).ConfigureAwait( false );
					var payload = container.Payload;
					if ( DevicesCache.TryGetValue( container.Topic, out var cacheItem ) && !cacheItem.IsExpired )
					{
						EnvironmentMeter.Update( payload.Temperature, payload.Humidity, payload.Pressure, cacheItem.Value.Location, cacheItem.Value.Hardware.Model );
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
						EnvironmentMeter.Update( payload.Temperature, payload.Humidity, payload.Pressure, newItem.Value.Location, newItem.Value.Hardware.Model );
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
