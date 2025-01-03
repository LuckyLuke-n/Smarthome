namespace LSoftware.Metrics.Abstractions
{
	public interface IMetricsLogger<in T> : IDisposable where T : IMetricsObject
	{
		/// <summary>
		/// Sends the metrics object into the buffer and from there to the metrics database.
		/// </summary>
		/// <param name="data"></param>
		void Send( T data );

		/// <summary>
		/// Sends the metrics onject into the database without buffering.
		/// </summary>
		/// <param name="data"></param>
		void SendInstant( T data );
	}
}