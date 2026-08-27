using Testcontainers.Mosquitto;

namespace Smarthome.AmbientCollector.Integration.Communication;

public sealed class MqttFixture : IAsyncLifetime
{
    private readonly MosquittoContainer _mqttContainer;
    
    public string Host => _mqttContainer.Hostname;
    public int Port => _mqttContainer.GetMappedPublicPort(1883);
    
    // MqttConfiguration expects mqtt://user:pass@host:port
    // By default Mosquitto might not have users, but our MqttConfiguration REQUIRES it in the string.
    // If we don't configure Mosquitto with auth, we can still use dummy credentials in the connection string
    // if Mosquitto is configured to allow anonymous.
    public string ConnectionString => $"mqtt://guest:guest@{Host}:{Port}";

    public MqttFixture()
    {
        _mqttContainer = new MosquittoBuilder("eclipse-mosquitto:latest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mqttContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mqttContainer.DisposeAsync();
    }
}
