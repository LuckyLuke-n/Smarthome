using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Communication.Mqtt.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LSoftware.Communication.Mqtt.Handler
{
	internal class MqttConnectionHandler : IMqttConnectionHandler
	{
		private bool disposedValue;

		private ConcurrentDictionary<string, MqttClientHandler> Clients { get; set; } = [];
		private MqttConfiguration MqttConfiguration { get; }
		private ILogger<MqttConnectionHandler> Logger { get; }
		private Func<MqttClientHandler> ClientFactory { get; }
		private static object Lock { get; } = new object();

		public MqttConnectionHandler( IOptions<MqttConfiguration> options, ILogger<MqttConnectionHandler> logger, Func<MqttClientHandler> clientFactory )
		{
			Logger = logger;
			ClientFactory = clientFactory;
			MqttConfiguration = options.Value;

			if ( !MqttConfiguration.IsConfigured )
				Logger.LogError( "MqttConfiguration is missing." );
		}

		public async Task<ISubscriber> GetSubscriber( string topic, CancellationToken cancellationToken = default )
		{
			lock ( Lock )
			{
				if ( Clients.TryGetValue( topic, out var client ) )
				{
					client.IncreaseCount();
					return client;
				}
			}

			var newClient = ClientFactory();
			await newClient.ConnectAsync().ConfigureAwait( false );
			await newClient.SubscribeAsync( topic ).ConfigureAwait( false );

			lock ( Lock )
				Clients.TryAdd( topic, newClient );

			return newClient;
		}

		public void DisconnectSubscriber( ISubscriber subscriber )
		{
			var client = subscriber as MqttClientHandler;
			client?.DecreaseCount();
			var disposed = client?.TryDispose() ?? false;

			if ( !disposed )
				return;

			lock ( Lock )
			{
				if ( !Clients.Remove( subscriber.Topic, out var _ ) )
					Logger.LogWarning( "Could not remove the client for topic {Topic} from the handled mqtt connecitons.", subscriber.Topic );
			}
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( !disposedValue )
			{
				if ( disposing )
				{
					// TODO dispose all connections
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose( disposing: true );
			GC.SuppressFinalize( this );
		}
	}
}
