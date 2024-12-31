namespace LSoftware.Communication.Mqtt.Configuration
{
	public class MqttConfiguration
	{
		public static string Section => "Mqtt";
		public string Host { get; set; } = string.Empty;
		public string Port { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string UseTls { get; set; } = false.ToString();

		public bool TlsEnabled => bool.Parse( UseTls );

		public bool IsConfigured => string.IsNullOrEmpty( Host ) ||
			string.IsNullOrEmpty( Port ) ||
			string.IsNullOrEmpty( Username ) ||
			string.IsNullOrEmpty( Password );
	}
}