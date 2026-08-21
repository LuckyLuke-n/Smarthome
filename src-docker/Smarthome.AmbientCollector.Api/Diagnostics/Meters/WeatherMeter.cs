using System.Diagnostics.Metrics;
using Smarthome.AmbientCollector.Api.Repositories.WeatherReport;

namespace Smarthome.AmbientCollector.Api.Diagnostics.Meters
{
	public static class WeatherMeter
	{
		public static string Name => "WeatherApi.Current";
		private static readonly Meter Meter = new( Name, "1.0" );
		private static readonly Gauge<double> TemperatureGauge;
		private static readonly Gauge<double> TemperatureApparentGauge;
		private static readonly Gauge<double> HumidityGauge;
		private static readonly Gauge<double> RainIntensityGauge;
		private static readonly Gauge<double> PressureGauge;
		private static readonly Gauge<double> WindSpeedGauge;

		static WeatherMeter()
		{
			// Initialize the metrics
			TemperatureGauge = Meter.CreateGauge<double>( "ambient.weather.current.temperature", "Cel", "Current temperature in Celsius" );
			TemperatureApparentGauge = Meter.CreateGauge<double>( "ambient.weather.current.temperatureApparent", "Cel", "Apparent temperature in Celsius" );
			HumidityGauge = Meter.CreateGauge<double>( "ambient.weather.current.humidity", "%", "Current humidity in %" );
			PressureGauge = Meter.CreateGauge<double>( "ambient.weather.current.pressure", "hPa", "Current ambient pressure in hPa" );
			RainIntensityGauge = Meter.CreateGauge<double>( "ambient.weather.current.rainIntensity", "mm/h", "Rain intensity in mm per hour." );
			WindSpeedGauge = Meter.CreateGauge<double>( "ambient.weather.current.windSpeed", "m/s", "Wind speed in meter per second." );
		}

		public static void Update( WeatherReport weatherReport, string location )
		{
			var tags = new KeyValuePair<string, object?>[]
			{
				new("location", location),
			};
			TemperatureGauge.Record( weatherReport.Temperature, tags );
			TemperatureApparentGauge.Record( weatherReport.TemperatureApparent, tags );
			HumidityGauge.Record( weatherReport.Humidity, tags );
			PressureGauge.Record( weatherReport.PressureSeaLevel, tags );
			RainIntensityGauge.Record( weatherReport.RainIntensity, tags );
			WindSpeedGauge.Record( weatherReport.WindSpeed, tags );
		}
	}
}
