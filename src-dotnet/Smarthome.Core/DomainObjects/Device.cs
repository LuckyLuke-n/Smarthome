using System.Text.Json.Serialization;

namespace Smarthome.Core.DomainObjects
{
	[Serializable]
	public class Device
	{
		public string Id { get; set; } = string.Empty;
		public string Hostname { get; set; } = string.Empty;
		public Hardware Hardware { get; set; } = new();
		public string Location { get; set; } = string.Empty;
		public string DataSource => $"{Hostname}/{Hardware.Type}".ToLower();
		[JsonConverter( typeof( JsonStringEnumConverter ) )]
		public State State { get; set; }

		public void SetAsNew()
		{
			Id = Guid.NewGuid().ToString();
		}

		public static string DateSourceToHostname( string dataSource ) => dataSource.Split( '/' )[ 0 ];

		public static HardwareType DataSourceToHardwareType( string dataSource )
		{
			if ( Enum.TryParse<HardwareType>( dataSource.Split( '/' )[ 1 ], true, out var value ) )
				return value;
			else
				return HardwareType.Unknown;
		}
	}
}
