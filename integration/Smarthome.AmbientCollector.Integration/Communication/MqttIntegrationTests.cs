using LSoftware.Communication.Mqtt.Configuration;
using LSoftware.Communication.Mqtt.Handler;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using Smarthome.AmbientCollector.Api.Monitoring.MessageBus;
using System.Text.Json;

namespace Smarthome.AmbientCollector.Integration.Communication;

public class MqttIntegrationTests : IClassFixture<MqttFixture>
{
    private readonly MqttFixture _fixture;

    public MqttIntegrationTests(MqttFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Subscriber_ShouldReceiveData_WhenDataIsPublished()
    {
        // Arrange
        var logger = NullLogger<MqttConnectionHandler>.Instance;
        var clientLogger = NullLogger<MqttClientHandler>.Instance;
        
        // Factory for MqttClientHandler
        Func<MqttClientHandler> clientFactory = () => new MqttClientHandler(_fixture.ConnectionString, clientLogger);
        
        using var connectionHandler = new MqttConnectionHandler(logger, clientFactory);
        var topic = "environmentsensor";
        var expectedSensorData = new Environmentsensor
        {
            Timestamp = "2026-08-27T21:49:00Z",
            Temperature = 22.5,
            Pressure = 1013.25,
            Humidity = 45.0,
            Location = "living-room",
            Sensor = "bme280"
        };
        var expectedMessage = JsonSerializer.Serialize(expectedSensorData);
        string? receivedMessage = null;
        var tcs = new TaskCompletionSource<string>();

        var subscriber = await connectionHandler.GetSubscriberAsync(topic, TestContext.Current.CancellationToken);
        subscriber.RegisterCallback((t, m) =>
        {
            receivedMessage = m;
            tcs.TrySetResult(m);
        });

        // Act
        await PublishMessageAsync(topic, expectedMessage);

        // Assert
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(10000, TestContext.Current.CancellationToken));
        Assert.Equal(tcs.Task, completedTask);
        Assert.Equal(expectedMessage, receivedMessage);
    }

    private async Task PublishMessageAsync(string topic, string payload)
    {
        var mqttFactory = new MqttClientFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var mqttConfiguration = new MqttConfiguration();
        MqttConfiguration.TryCreate(_fixture.ConnectionString, out mqttConfiguration);

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttConfiguration.Host, mqttConfiguration.Port)
            .WithCredentials(mqttConfiguration.Username, mqttConfiguration.Password)
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        await mqttClient.PublishAsync(message, CancellationToken.None);
        await mqttClient.DisconnectAsync();
    }
}
