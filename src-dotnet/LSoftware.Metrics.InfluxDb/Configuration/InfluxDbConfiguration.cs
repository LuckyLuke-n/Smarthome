namespace LSoftware.Metrics.InfluxDb.Configuration
{
	public class InfluxDbConfiguration
	{
		public static string Section => "InfluxDb";
		public string Url { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
		public string Buffer { get; set; } = string.Empty;

		public int CacheSize => GetCacheSize();


		public static string UrlEnvVar => $"SMARTHOME_{Section}__{nameof( Url )}";
		public static string TokenEnvVar => $"SMARTHOME_{Section}__{nameof( Token )}";

		private int GetCacheSize()
		{
			if ( int.TryParse( Buffer, out int buffer ) )
				return buffer;
			else
				return 1;
		}
	}
}
