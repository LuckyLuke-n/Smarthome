using System.Text.Json;
using LSoftware.Communication.Abstractions.MessageBus;
using Smarthome.AmbientCollector.Api.Diagnostics.Meters;

namespace Smarthome.AmbientCollector.Api.Monitoring.MessageBus
{
	public class DeviceMonitor : IHostedService
	{
		private IConnectionHandler ConnectionHandler { get; }
		private ILogger<DeviceMonitor> Logger { get; }
		private ISubscriber Subscriber { get; set; } = null!;

		public DeviceMonitor( IConnectionHandler connectionHandler,
			ILogger<DeviceMonitor> logger )
		{
			ConnectionHandler = connectionHandler;
			Logger = logger;
		}

		public async Task StartAsync( CancellationToken cancellationToken )
		{
			Subscriber = await ConnectionHandler.GetSubscriberAsync( "environmentsensor", cancellationToken ).ConfigureAwait( false );
			Subscriber.RegisterCallback( Received );
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			ConnectionHandler.Dispose();
			await Task.CompletedTask.ConfigureAwait( false );
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
				
				EnvironmentMeter.Update( payload.Temperature, payload.Humidity, payload.Pressure, payload.Location, payload.Sensor );
			}
			catch ( JsonException ex )
			{
				Logger.LogWarning( ex, "Could not deserialize for {Topic}", topic );
			}
		}
	}
}