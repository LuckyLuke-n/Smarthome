using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, long> LastReceived { get; } = [];
        
#if DEBUG
        private TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);
#else
        private TimeSpan Timeout { get; } = TimeSpan.FromMinutes(5);
#endif
        
        public DeviceMonitor(IConnectionHandler connectionHandler,
            ILogger<DeviceMonitor> logger)
        {
            ConnectionHandler = connectionHandler;
            Logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Subscriber = await ConnectionHandler.GetSubscriberAsync("environmentsensor", cancellationToken)
                .ConfigureAwait(false);
            Subscriber.RegisterCallback(Received);

            _ = Task.Run(async () => await CheckSensorHealthAsync(cancellationToken).ConfigureAwait(false),
                cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            ConnectionHandler.Dispose();
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task CheckSensorHealthAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var item in LastReceived)
                {
                    if (DateTime.UtcNow.Ticks - item.Value > Timeout.Ticks)
                    {
                        EnvironmentMeter.TryRemoveSensor(item.Key);
                        LastReceived.TryRemove(item.Key, out _);
                        Logger.LogWarning("Sensor {Sensor} did not send data the last {Timeout} seconds.", item.Key, Timeout.TotalSeconds);
                    }
                }

                await Task.Delay(Timeout, cancellationToken);
            }
        }

        private void Received(string topic, string data)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<Environmentsensor>(data);

                if (payload is null)
                {
                    Logger.LogWarning("Deserializing payload for {Topic} returned null.", topic);
                    return;
                }

                LastReceived.AddOrUpdate(payload.Location, _ => DateTime.UtcNow.Ticks, (_, _) => DateTime.UtcNow.Ticks);
                EnvironmentMeter.Update(payload.Temperature, payload.Humidity, payload.Pressure, payload.Location,
                    payload.Sensor);
            }
            catch (JsonException ex)
            {
                Logger.LogWarning(ex, "Could not deserialize for {Topic}", topic);
            }
        }
    }
}