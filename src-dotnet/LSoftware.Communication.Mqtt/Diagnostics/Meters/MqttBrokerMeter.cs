using System.Diagnostics.Metrics;

namespace LSoftware.Communication.Mqtt.Diagnostics.Meters;

public static class MqttBrokerMeter
{
    public static string Name => "MqttConnection.Broker";
    private static readonly Meter Meter = new(Name, "1.0");

    private static readonly Counter<int> ReconnectCounter;

    static MqttBrokerMeter()
    {
        ReconnectCounter = Meter.CreateCounter<int>("mqtt_communication.reconnect_attempts", "", "Number of reconnect attempts to the MQTT broker");
    }

    internal static void ReconnectAttempt(string host)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("mqtt.url", host),
        };

        ReconnectCounter.Add(1, tags);
    }
}