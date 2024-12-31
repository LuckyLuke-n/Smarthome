namespace LSoftware.Communication.Abstractions.MessageBus
{
	public interface ISubscriber
	{
		string Topic { get; }
		void RegisterCallback( Action<string> callback );
	}
}
