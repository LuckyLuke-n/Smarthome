using Smarthome.Core.DomainObjects;

namespace Smarthome.Api.Repositories.Devices
{
	public interface IDeviceRepository
	{
		Task<RepositoryResponse<Device, DeviceRepositoryFailResponse>> CreateAsync( Device device, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<IEnumerable<Device>, DeviceRepositoryFailResponse>> ReadAllAsync( CancellationToken cancellationToken = default );
		Task<RepositoryResponse<Device, DeviceRepositoryFailResponse>> ReadAsync( Guid id, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<Device, DeviceRepositoryFailResponse>> UpdateAsync( Device device, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<DeviceRepositoryFailResponse>> DeleteAsync( Guid id, CancellationToken cancellationToken = default );
	}
}
