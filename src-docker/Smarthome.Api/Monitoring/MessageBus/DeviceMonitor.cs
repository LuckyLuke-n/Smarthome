using LSoftware.Communication.Abstractions.MessageBus;
using System.Text.Json;

namespace Smarthome.Api.Monitoring.MessageBus
{
	public class DeviceMonitor : IHostedService
	{
		private IConnectionHandler ConnectionHandler { get; }

		public DeviceMonitor( IConnectionHandler connectionHandler )
		{
			ConnectionHandler = connectionHandler;
		}


		public async Task StartAsync( CancellationToken cancellationToken )
		{
			var subscriber = await ConnectionHandler.GetSubscriber( $"{Environment.MachineName.ToLower()}/environmentsensor", cancellationToken ).ConfigureAwait( false );
			subscriber.RegisterCallback( Received );

		}

		private void Received( string data )
		{		
			var payload = JsonSerializer.Deserialize<Payload>( data );
		}

		public async Task StopAsync( CancellationToken cancellationToken )
		{
			await Task.CompletedTask.ConfigureAwait( false );
			ConnectionHandler.Dispose();
		}
	}
}
