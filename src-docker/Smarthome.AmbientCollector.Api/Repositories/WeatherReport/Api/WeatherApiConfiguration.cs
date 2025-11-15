namespace Smarthome.AmbientCollector.Api.Repositories.WeatherReport.Api
{
	public class WeatherApiConfiguration
	{
		public static string Section => "WeatherApi";

		public string Endpoint { get; set; } = string.Empty;
		public string ApiKey { get; set; } = string.Empty;
		public int RefreshIntervalInMinutes {  get; set; }
	}
}
