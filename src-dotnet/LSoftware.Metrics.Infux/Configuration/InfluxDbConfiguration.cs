namespace LSoftware.Metrics.Infux.Configuration
{
	public class InfluxDbConfiguration
	{
		public static string Section => "InfluxDB";
		public string Url { get; set; } = string.Empty;
		public string Token { get;set; } = string.Empty;

		public static string UrlEnvVar => $"SMARTHOME_{Section}__{nameof( Url )}";
		public static string TokenEnvVar => $"SMARTHOME_{Section}__{nameof( Token )}";
	}
}
