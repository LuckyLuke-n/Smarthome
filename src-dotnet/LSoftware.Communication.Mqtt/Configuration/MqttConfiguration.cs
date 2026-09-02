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
		
		public string ConnectionString { get; private set; } = string.Empty;

		public static bool TryCreate(string connectionString, out MqttConfiguration configuration )
		{
			configuration = new MqttConfiguration();

			// check for a valid connection string
			if (string.IsNullOrEmpty(connectionString)
			    || (!connectionString.StartsWith("mqtts://") && !connectionString.StartsWith("mqtt://")))
			{
				return false;
			}
			
			configuration.ConnectionString = connectionString;
			return true;
		}
	}
}