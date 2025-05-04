namespace LSoftware.Communication.Abstractions.MessageBus
{
	public interface IConnectionHandler : IDisposable
	{
		/// <summary>
		/// Creates a new subscriber or returns the existing one for that connection.
		/// </summary>
		/// <returns>Returns the <see cref="ISubscriber"/>.</returns>
		Task<ISubscriber> GetSubscriberAsync( string topic, CancellationToken cancellationToken = default );
		/// <summary>
		/// Disposes the actual client when all instances are stopped.
		/// </summary>
		/// <param name="subscriber"></param>
		void DisconnectSubscriber( ISubscriber subscriber );
	}
}
