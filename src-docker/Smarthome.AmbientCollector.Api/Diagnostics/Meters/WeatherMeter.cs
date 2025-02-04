using Smarthome.AmbientCollector.Api.Repositories.WeatherReport;
using System.Diagnostics.Metrics;

namespace Smarthome.AmbientCollector.Api.Diagnostics.Meters
{
	public static class WeatherMeter
	{
		public static string Name => "WeatherApi.Current";
		private static readonly Meter _meter = new( Name, "1.0" );
		private static readonly Gauge<double> _temperatureGauge;
		private static readonly Gauge<double> _temperatureApparentGauge;
		private static readonly Gauge<double> _humidityGauge;
		private static readonly Gauge<double> _rainIntensityGauge;
		private static readonly Gauge<double> _pressureGauge;
		private static readonly Gauge<double> _windSpeedGauge;

		static WeatherMeter()
		{
			// Initialize the metrics
			_temperatureGauge = _meter.CreateGauge<double>( "weather.current.temperature", "degC", "Current temperature in Celsius" );
			_temperatureApparentGauge = _meter.CreateGauge<double>( "weather.current.temperatureApparent", "degC", "Apparent temperature in Celsius" );
			_humidityGauge = _meter.CreateGauge<double>( "weather.current.humidity", "%", "Current humidity in %" );
			_pressureGauge = _meter.CreateGauge<double>( "weather.current.pressure", "hPa", "Current ambient pressure in hPa" );
			_rainIntensityGauge = _meter.CreateGauge<double>( "weather.current.rainIntensity", "mm/h", "Rain intensity in mm per hour." );
			_windSpeedGauge = _meter.CreateGauge<double>( "weather.current.windSpeed", "m/s", "Wind speed in meter per second." );
		}

		public static void Update( WeatherReport weatherReport, string location )
		{
			var tags = new KeyValuePair<string, object?>[]
			{
				new("location", location),
			};
			_temperatureGauge.Record( weatherReport.Temperature, tags );
			_temperatureApparentGauge.Record( weatherReport.TemperatureApparent, tags );
			_humidityGauge.Record( weatherReport.Humidity, tags );
			_pressureGauge.Record( weatherReport.PressureSurfaceLevel, tags );
			_rainIntensityGauge.Record( weatherReport.RainIntensity, tags );
			_windSpeedGauge.Record( weatherReport.WindSpeed, tags );
		}
	}
}
