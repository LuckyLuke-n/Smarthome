using InfluxDB.Client.Writes;

namespace LSoftware.Metrics.Abstractions
{
	public interface IMetricsObject
	{
		string MeasurementName { get; }
		PointData ToInfluxDbDataPoint();
	}
}