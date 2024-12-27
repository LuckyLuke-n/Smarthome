using Smarthome.Core.DomainObjects;

namespace Smarthome.Api.Repositories.Devices
{
	public interface IDeviceRepository
	{
		Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> CreateAsync( Device device, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> ReadAsync( int id, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> UpdateAsync( Device device, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> DeleteAsync( int id, CancellationToken cancellationToken = default );
	}
}
