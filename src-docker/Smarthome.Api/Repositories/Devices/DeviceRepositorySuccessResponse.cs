using Smarthome.Core.DomainObjects;

namespace Smarthome.Api.Repositories.Devices
{
	public class DeviceRepositorySuccessResponse
	{
		public Device Device { get; set; }

		public DeviceRepositorySuccessResponse()
		{
			Device = new();
		}

		public DeviceRepositorySuccessResponse( Device device )
		{
			Device = device;
		}
	}
}
