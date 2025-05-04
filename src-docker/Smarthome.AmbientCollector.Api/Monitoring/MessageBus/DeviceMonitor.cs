using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using LSoftware.Communication.Abstractions.MessageBus;
using Smarthome.AmbientCollector.Api.Diagnostics.Meters;

namespace Smarthome.AmbientCollector.Api.Monitoring.MessageBus
{
	public class DeviceMonitor : IHostedService
	{
		private IConnectionHandler ConnectionHandler { get; }
		private ILogger<DeviceMonitor> Logger { get; }

		private BufferBlock<Environmentsensor> MetricsBuffer { get; } = new();

		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private ConcurrentDictionary<string, ISubscriber> Sources { get; } = [];

		public DeviceMonitor( IConnectionHandler connectionHandler,
			ILogger<DeviceMonitor> logger )
		{
			ConnectionHandler = connectionHandler;
			Logger = logger;
		}

		public async Task StartAsync( CancellationToken cancellationToken )
		{
			var subscriber = await ConnectionHandler.GetSubscriber( "environmentsensor", CancellationTokenSource.Token ).ConfigureAwait( false );
			subscriber.RegisterCallback( Received );
			await Task.Run( WorkOnMetricsBufferAsync, cancellationToken ).ConfigureAwait( false );
		}

		private async Task WorkOnMetricsBufferAsync()
		{
			while ( !CancellationTokenSource.IsCancellationRequested )
			{
				try
				{				
					var payload = await MetricsBuffer.ReceiveAsync( CancellationTokenSource.Token ).ConfigureAwait( false );
					EnvironmentMeter.Update( payload.Temperature, payload.Humidity, payload.Pressure, payload.Location, payload.Sensor );
				}
				catch ( Exception ex )
				{
					Logger.LogInformation( ex, "MetricsBuffer was cancelled." );
				}
			}
		}
		
		private void Received( string topic, string data )
		{
			try
			{
				var payload = JsonSerializer.Deserialize<Environmentsensor>( data );

				if ( payload is null )
				{
					Logger.LogWarning( "Deserializing payload for {Topic} returned null.", topic );
					return;
				}
				
				MetricsBuffer.Post( payload );
			}
			catch ( JsonException ex )
			{
				Logger.LogWarning( ex, "Could not deserialize for {Topic}", topic );
			}
		}

		public async Task StopAsync( CancellationToken cancellationToken )
		{
			await CancellationTokenSource.CancelAsync().ConfigureAwait( false );
			ConnectionHandler.Dispose();
		}
	}
}