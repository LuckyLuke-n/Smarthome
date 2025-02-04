using System.Text.Json.Serialization;

namespace Smarthome.AmbientCollector.Api.Monitoring.MessageBus
{
	[Serializable]
	public class Payload
	{
		[JsonPropertyName("timestamp")]
		public string Timestamp { get; set; } = string.Empty;

		[JsonPropertyName( "temperature" )]
		public double Temperature { get; set; }

		[JsonPropertyName( "pressure" )]
		public double Pressure { get; set; }

		[JsonPropertyName( "humidity" )]
		public double Humidity { get; set; }
	}
}
