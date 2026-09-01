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
        private static Lock Lock { get; } = new();

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

        public async Task DisconnectSubscriberAsync(ISubscriber subscriber)
        {
            if (subscriber is not MqttClientHandler client)
                return;

            client.DecreaseCount();
            var disposed = await client.TryDestroyAsync().ConfigureAwait(false);

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

        public async ValueTask DisposeAsync()
        {
            if (!_disposedValue)
            {
                foreach (var client in Clients.Values)
                    await client.DisposeAsync().ConfigureAwait(false);
            }

            _disposedValue = true;
        }
    }
}