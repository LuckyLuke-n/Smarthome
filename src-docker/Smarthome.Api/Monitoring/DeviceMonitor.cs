using LSoftware.Communication.Abstractions.MessageBus;

namespace Smarthome.Api.Monitoring
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

		private void Received( byte[] data )
		{
			if ( data.Length > 0 )
				Console.WriteLine( $"received {data.Length} bytes." );
		}

		public async Task StopAsync( CancellationToken cancellationToken )
		{
			await Task.CompletedTask.ConfigureAwait( false );
			ConnectionHandler.Dispose();
		}
	}
}
