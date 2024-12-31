using System.Text.Json.Serialization;

namespace Smarthome.Core.DomainObjects
{
	[Serializable]
	public class Hardware
	{
		public string Model { get; set; } = string.Empty;
		[JsonConverter( typeof( JsonStringEnumConverter ) )]
		public HardwareType Type { get; set; }
	}
}