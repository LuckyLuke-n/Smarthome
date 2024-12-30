using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Smarthome.Core.DomainObjects
{
	[Serializable]
	public class Device
	{
		public string Key { get; set; } = string.Empty;
		public string Hostname { get; set; } = string.Empty;
		public Hardware Hardware { get; set; } = new();
		public string Location { get; set; } = string.Empty;
		public string DataSource { get; set; } = string.Empty;
		[JsonConverter( typeof( JsonStringEnumConverter ) )]
		public State State { get; set; }

		public void SetAsNew()
		{
			Key = Guid.NewGuid().ToString();
			DataSource = $"{Hostname}/{Hardware.Type}".ToLower();
		}
	}
}
