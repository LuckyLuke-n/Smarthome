namespace LSoftware.Communication.Mqtt.Configuration
{
	public class MqttConfiguration
	{
		public static string Section => "Mqtt";
		public string Host => ConnectionString.Split('@')[1].Split(':')[0];
		public int Port => Convert.ToInt32( ConnectionString.Split('@')[1].Split(':')[1] );
		public string Username => ConnectionString.Split('@')[0].Split("//")[1].Split(':')[0];
		public string Password => ConnectionString.Split('@')[0].Split("//")[1].Split(':')[1];
		public bool TlsEnabled => ConnectionString.StartsWith("mqtts://");
		
		private string ConnectionString { get; } = Environment.GetEnvironmentVariable( "ConnectionStrings__smarthome-mqtt" ) ?? string.Empty;

		public bool IsConfigured => !string.IsNullOrEmpty(ConnectionString)
			&& ( ConnectionString.StartsWith("mqtts://") ||  ConnectionString.StartsWith("mqtt://") );
	}
}