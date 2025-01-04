using LSoftware.Repository.Abstractions;
using Smarthome.Core.DomainObjects;

namespace Smarthome.Api.Repositories.Devices
{
	public interface IDeviceRepository
	{
		Task<RepositoryResponse<Device, RepositoryFailResponse>> CreateAsync( Device device, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<IEnumerable<Device>, RepositoryFailResponse>> ReadAllAsync( CancellationToken cancellationToken = default );
		Task<RepositoryResponse<IEnumerable<Device>, RepositoryFailResponse>> ReadReadyAndSendingAsync( CancellationToken cancellationToken = default );
		Task<RepositoryResponse<Device, RepositoryFailResponse>> ReadAsync( Guid id, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<Device, RepositoryFailResponse>> ReadAsync( string hostname, HardwareType type, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<Device, RepositoryFailResponse>> UpdateAsync( Device device, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<RepositoryFailResponse>> DeleteAsync( Guid id, CancellationToken cancellationToken = default );
	}
}
