using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using LSoftware.Metrics.Abstractions;

namespace Smarthome.AmbientCollector.Api.Monitoring.WeatherData
{
	public class WeatherReportData : IMetricsObject
	{
		public string MeasurementName => "weather_report";

		public string Location { get; set; } = string.Empty;
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public float Temperature { get; set; }
		public float TemperatureApparent { get; set; }
		public int Humidity { get; set; }
		public double RainIntensity { get; set; }
		public float PressureSurfaceLevel { get; set; }
		public float WindSpeed { get; set; }

		private DateTime Timestamp { get; } = DateTime.UtcNow;

		public PointData ToInfluxDbDataPoint()
		{
			var point = PointData
				.Measurement( MeasurementName )
				.Tag( nameof( Location ), Location )
				.Field( nameof( Temperature ), Temperature )
				.Field( nameof( TemperatureApparent ), TemperatureApparent )
				.Field( nameof( Humidity ), Humidity )
				.Field( nameof( RainIntensity ), RainIntensity )
				.Field( nameof( PressureSurfaceLevel ), PressureSurfaceLevel )
				.Field( nameof( WindSpeed ), WindSpeed )
				.Timestamp( Timestamp, WritePrecision.Ns );

			return point;
		}
	}
}
