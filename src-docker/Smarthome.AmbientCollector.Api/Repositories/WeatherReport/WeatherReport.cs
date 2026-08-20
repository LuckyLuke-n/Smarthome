namespace Smarthome.AmbientCollector.Api.Repositories.WeatherReport
{
	public class WeatherReport
	{
		public string Location { get; set; } = string.Empty;
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public double Temperature { get; set; }
		public double TemperatureApparent { get; set; }
		public double Humidity { get; set; }
		public double RainIntensity { get; set; }
		public double PressureSurfaceLevel { get; set; }
		public double PressureSeaLevel { get; set; }
		public double WindSpeed { get; set; }
	}
}
