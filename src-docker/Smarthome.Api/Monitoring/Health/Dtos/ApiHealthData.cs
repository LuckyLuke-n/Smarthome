using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using LSoftware.Metrics.Abstractions;

namespace Smarthome.Api.Monitoring.Health.Dtos
{
	public class ApiHealthData : IMetricsObject
	{
		public string MeasurementName => "api_health";

		public string Database { get; set; } = string.Empty;
		public string Grafana { get; set; } = string.Empty;
		public string MessageBus { get; set; } = string.Empty;

		private string ApiVersion { get; } = ServiceConstants.Version;
		private DateTime Timestamp { get; } = DateTime.UtcNow;

		public PointData ToInfluxDbDataPoint()
		{
			var point = PointData
				.Measurement( MeasurementName )
				.Field( nameof( Database ), Database )
				.Field( nameof( Grafana ), Grafana )
				.Field( nameof( MessageBus ), MessageBus )
				.Field( nameof( ApiVersion ), ApiVersion )
				.Timestamp( Timestamp, WritePrecision.Ns );

			return point;
		}
	}
}