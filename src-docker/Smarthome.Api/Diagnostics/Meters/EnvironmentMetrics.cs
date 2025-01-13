using OpenTelemetry;
using OpenTelemetry.Metrics;
using Smarthome.Core.DomainObjects;
using System.Diagnostics.Metrics;

namespace Smarthome.Api.Diagnostics.Meters
{
	public class EnvironmentMetrics
	{
		private static Meter _meter = new( "EnvironmentSensor.Readings", "1.0" );
		private static Gauge<double> _temperatureGauge;

		static EnvironmentMetrics()
		{
			// Initialize the metrics
			_temperatureGauge = _meter.CreateGauge<double>( "temperature", "Current temperature in Celsius" );
		}

		public void Update( double temperature, double humidity, string location, string sensorModel )
		{
			var tags = new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("location", location),
				new KeyValuePair<string, object>("sensor_model", sensorModel)
			};
			_temperatureGauge.Record( temperature, tags );
		}
	}
}
