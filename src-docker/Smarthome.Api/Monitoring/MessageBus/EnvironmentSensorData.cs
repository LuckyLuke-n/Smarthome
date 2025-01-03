using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using LSoftware.Metrics.Abstractions;
using Smarthome.Core.DomainObjects;
using System.Globalization;

namespace Smarthome.Api.Monitoring.MessageBus
{
	public class EnvironmentSensorData : IMetricsObject
	{
		public string MeasurementName { get; } = "environement_sensor";

		public DateTime Timestamp { get; }
		public double Temperature { get; }
		public double Humidity { get; }
		public double Pressure { get; }

		private string Location { get; }
		private string Hostname { get; }
		private string DeviceModel { get; }

		public EnvironmentSensorData( Payload payload, Device device )
		{
			if ( DateTime.TryParse( payload.Timestamp, CultureInfo.InvariantCulture, out var timestamp ) )
			{
				Timestamp = timestamp;
				Timestamp = DateTime.Parse( payload.Timestamp, CultureInfo.InvariantCulture );
				Temperature = payload.Temperature;
				Humidity = payload.Humidity;
				Pressure = payload.Pressure;
				Location = device.Location;
				Hostname = device.Hostname;
				DeviceModel = device.Hardware.Model;
			}
			else
				throw new InvalidOperationException( "Timestamp could not be parsed." );

		}

		public PointData ToInfluxDbDataPoint()
		{
			var point = PointData
				.Measurement( MeasurementName )
				.Tag( nameof( Location ), Location )
				.Tag( nameof( Hostname ), Hostname )
				.Tag( nameof( DeviceModel ), DeviceModel )
				.Field( nameof( Temperature ), Temperature )
				.Field( nameof( Humidity ), Humidity )
				.Field( nameof( Pressure ), Pressure )
				.Timestamp( Timestamp, WritePrecision.Ns );

			return point;
		}
	}
}
