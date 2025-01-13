using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using LSoftware.Metrics.Abstractions;

namespace Smarthome.Api.Monitoring.Health.Dtos
{
	public class ApiHealthData : IMetricsObject
	{
		public string MeasurementName => "api_health";

		public string InfluxDb { get; set; } = string.Empty;
		public string Grafana { get; set; } = string.Empty;
		public string RabbitMq { get; set; } = string.Empty;

		private DateTime Timestamp { get; } = DateTime.UtcNow;

		public PointData ToInfluxDbDataPoint()
		{
			var point = PointData
				.Measurement( MeasurementName )
				.Field( nameof( InfluxDb ), InfluxDb )
				.Field( nameof( Grafana ), Grafana )
				.Field( nameof( RabbitMq ), RabbitMq )
				.Timestamp( Timestamp, WritePrecision.Ns );

			return point;
		}
	}
}