namespace Smarthome.Api.Repositories.WeatherReport.Api
{
	public class WeatherApiConfiguration
	{
		public static string Section => "WeatherApi";

		public string Endpoint { get; set; } = string.Empty;
		public string ApiKey { get; set; } = string.Empty;
		public string RefreshIntervalInMinutes {  get; set; } = string.Empty;

		public int IntervalInMinutes => int.Parse( RefreshIntervalInMinutes );
	}
}
