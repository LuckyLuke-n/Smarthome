using LSoftware.Communication.Abstractions.MessageBus;
using MQTTnet;

namespace LSoftware.Communication.Mqtt.Handler
{
	internal interface IMqttConnectionHandler : IDisposable
	{
		/// <summary>
		/// Creates a new mqtt client or returns the existing one for that connection.
		/// </summary>
		/// <returns>Returns the <see cref="MqttClient"/>.</returns>
		Task<ISubscriber> GetSubscriber( string topic, CancellationToken cancellationToken = default );
		/// <summary>
		/// Disposes the actual client when all instances are stopped.
		/// </summary>
		/// <param name="subscriber"></param>
		void DisconnectSubscriber( ISubscriber subscriber );
	}
}
