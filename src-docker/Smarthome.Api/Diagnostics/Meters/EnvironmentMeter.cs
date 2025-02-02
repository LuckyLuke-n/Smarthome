using System.Diagnostics.Metrics;

namespace Smarthome.Api.Diagnostics.Meters
{
	public static class EnvironmentMeter
	{
		public static string Name = "EnvironmentSensor.Readings";
		private static readonly Meter _meter = new( Name, "1.0" );
		private static readonly Gauge<double> _temperatureGauge;
		private static readonly Gauge<double> _humidityGauge;
		private static readonly Gauge<double> _pressureGauge;

		static EnvironmentMeter()
		{
			// Initialize the metrics
			_temperatureGauge = _meter.CreateGauge<double>( "temperature", "degC", "Current temperature in Celsius" );
			_humidityGauge = _meter.CreateGauge<double>( "humidity", "%", "Current humidity in %" );
			_pressureGauge = _meter.CreateGauge<double>( "pressure", "hPa", "Current ambient pressure in hPa" );
		}

		public static void Update( double temperature, double humidity, double pressure, string location, string sensorModel )
		{
			var tags = new KeyValuePair<string, object?>[]
			{
				new("location", location),
				new("sensor_model", sensorModel)
			};
			_temperatureGauge.Record( temperature, tags );
			_humidityGauge.Record( humidity, tags );
			_pressureGauge.Record( pressure, tags );
		}
	}
}
