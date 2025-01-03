namespace LSoftware.Metrics.Abstractions
{
	public interface IMetricsLogger<in T> : IDisposable where T : IMetricsObject
	{
		void Send( T data );
	}
}