using LSoftware.Communication.Abstractions.MessageBus;

namespace LSoftware.Communication.Mqtt
{
	internal class MqttSubscriber : ISubscriber
	{
		private bool disposedValue;

		public MqttSubscriber()
		{
		}

		public void RegisterCallback( Action<byte[]> callback )
		{
			throw new NotImplementedException();
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( !disposedValue )
			{
				if ( disposing )
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose( disposing: true );
			GC.SuppressFinalize( this );
		}
	}
}
