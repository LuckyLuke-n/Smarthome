using LSoftware.Repository.Abstractions;
using Smarthome.Core.DomainObjects;

namespace Smarthome.Api.Repositories.Locations
{
	public interface ILocationRepository
	{
		Task<RepositoryResponse<Location, RepositoryFailResponse>> CreateAsync( Location location, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<IEnumerable<Location>, RepositoryFailResponse>> ReadAllAsync( CancellationToken cancellationToken = default );
		Task<RepositoryResponse<Location, RepositoryFailResponse>> ReadAsync( Guid id, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<Location, RepositoryFailResponse>> UpdateAsync( Location location, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<RepositoryFailResponse>> DeleteAsync( Guid id, CancellationToken cancellationToken = default );
	}
}
