using System.Collections.Concurrent;
using LSoftware.Communication.Abstractions.MessageBus;
using Microsoft.Extensions.Logging;

namespace LSoftware.Communication.Mqtt.Handler
{
    public class MqttConnectionHandler : IConnectionHandler
    {
        private bool _disposedValue;

        private ConcurrentDictionary<string, MqttClientHandler> Clients { get; set; } = [];
        private ILogger<MqttConnectionHandler> Logger { get; }
        private Func<MqttClientHandler> ClientFactory { get; }
        private static object Lock { get; } = new object();

        public MqttConnectionHandler(ILogger<MqttConnectionHandler> logger, Func<MqttClientHandler> clientFactory)
        {
            Logger = logger;
            ClientFactory = clientFactory;
        }

        public async Task<ISubscriber> GetSubscriberAsync(string topic, CancellationToken cancellationToken = default)
        {
            lock (Lock)
            {
                if (Clients.TryGetValue(topic, out var client))
                {
                    client.IncreaseCount();
                    return client;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            var newClient = ClientFactory();
            var connected = await newClient.ConnectAsync().ConfigureAwait(false);

            if (!connected)
                Logger.LogError("Could not connect the subscriber to the mqtt broker.");
            else
                Logger.LogInformation("Subscriber connected to the mqtt broker.");

            await newClient.SubscribeAsync(topic).ConfigureAwait(false);
            newClient.IncreaseCount();

            lock (Lock)
                Clients.TryAdd(topic, newClient);

            return newClient;
        }

        public void DisconnectSubscriber(ISubscriber subscriber)
        {
            var client = subscriber as MqttClientHandler;
            client?.DecreaseCount();
            var disposed = client?.TryDispose() ?? false;

            if (!disposed)
                return;

            lock (Lock)
            {
                if (!Clients.Remove(subscriber.Topic, out var _))
                    Logger.LogWarning(
                        "Could not remove the client for topic {Topic} from the handled mqtt connecitons.",
                        subscriber.Topic);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var client in Clients.Values)
                        client.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}