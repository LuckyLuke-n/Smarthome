namespace Smarthome.Api.Repositories.WeatherReport
{
	public class WeatherReport
	{
		public string Location { get; set; } = string.Empty;
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public float Temperature { get; set; }
		public float TemperatureApparent { get; set; }
		public int Humidity { get; set; }
		public int PrecipitationProbability { get; set; }
		public float PressureSurfaceLevel { get; set; }
		public float WindSpeed { get; set; }
	}
}
