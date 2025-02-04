using Smarthome.Core.DomainObjects;

namespace Smarthome.AmbientCollector.Api.Monitoring.MessageBus.Helpers
{
	internal class DeviceCacheItem
	{
		public Device Value { get; }
		public DateTime CreateAt { get; } = DateTime.UtcNow;

		public TimeSpan ExpirationTime { get; } = TimeSpan.FromMinutes( 5 );
		public bool IsExpired => ( DateTime.UtcNow - CreateAt ) > ExpirationTime;

		public DeviceCacheItem( Device device )
		{
			Value = device;
		}
	}
}