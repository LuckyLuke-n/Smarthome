using LSoftware.Communication.Abstractions.MessageBus;
using Smarthome.Api.Repositories.Devices;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Smarthome.Api.Monitoring.MessageBus
{
	public class DeviceMonitor : IHostedService
	{
		private IConnectionHandler ConnectionHandler { get; }
		private IDeviceRepository DeviceRepository { get; }
		private ILogger<DeviceMonitor> Logger { get; }

		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private Timer DevicesTimer { get; }
		private ConcurrentDictionary<string, ISubscriber> Sources { get; } = [];

		public DeviceMonitor( IConnectionHandler connectionHandler, IDeviceRepository deviceRepository, ILogger<DeviceMonitor> logger )
		{
			ConnectionHandler = connectionHandler;
			DeviceRepository = deviceRepository;
			Logger = logger;
			DevicesTimer = new( CrawlDeviceRepositoryAsync, null, int.MaxValue, int.MaxValue );
		}

		public async Task StartAsync( CancellationToken cancellationToken )
		{
			DevicesTimer.Change( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 30 ) );
			await Task.CompletedTask.ConfigureAwait( false );
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

			foreach ( var device in devices )
			{
				if ( !Sources.ContainsKey( device.DataSource ) )
				{
					var subscriber = await ConnectionHandler.GetSubscriber( device.DataSource, CancellationTokenSource.Token ).ConfigureAwait( false );
					subscriber.RegisterCallback( Received );

					Sources.TryAdd( device.DataSource, subscriber );
				}
			}

			var unsubscribe = Sources.Where( s => !devices.Select( d => d.DataSource ).Contains( s.Key ) );

			foreach ( var device in unsubscribe )
				ConnectionHandler.DisconnectSubscriber( device.Value );
		}

		private void Received( string topic, string data )
		{
			var top = topic;
			var payload = JsonSerializer.Deserialize<Payload>( data );
		}

		public async Task StopAsync( CancellationToken cancellationToken )
		{
			await CancellationTokenSource.CancelAsync().ConfigureAwait( false );
			await DevicesTimer.DisposeAsync().ConfigureAwait( false );
			ConnectionHandler.Dispose();
		}
	}
}
