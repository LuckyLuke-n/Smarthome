namespace LSoftware.Communication.Mqtt.Configuration
{
	public class MqttConfiguration
	{
		public static string Section => "Mqtt";
		public string Host => ConnectionString.Host;
		public int Port => ConnectionString.Port;
		public string Username => ConnectionString.UserInfo.Split(':')[0];
		public string Password => ConnectionString.UserInfo.Split(':')[1];
		public bool TlsEnabled => string.Equals( ConnectionString.Scheme, "mqtts", StringComparison.InvariantCultureIgnoreCase) ? true : false;
		
		private Uri ConnectionString { get; } = new (Environment.GetEnvironmentVariable( "ConnectionStrings__smarthome-mqtt" ) ?? string.Empty );
		
		public bool IsConfigured => !string.IsNullOrEmpty( Host ) ||
			!string.IsNullOrEmpty( Username ) ||
			!string.IsNullOrEmpty( Password );
	}
}