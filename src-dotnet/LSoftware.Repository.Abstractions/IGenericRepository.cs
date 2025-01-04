namespace LSoftware.Repository.Abstractions
{
	public interface IGenericRepository<T> where T : class, IEntity
	{
		Task<RepositoryResponse<T, RepositoryFailResponse>> CreateAsync( T entity, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<T, RepositoryFailResponse>> ReadAsync( Guid id, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<IEnumerable<T>, RepositoryFailResponse>> ReadAllAsync( CancellationToken cancellationToken = default );
		Task<RepositoryResponse<RepositoryFailResponse>> DeleteAsync( Guid id, CancellationToken cancellationToken = default );
		Task<RepositoryResponse<T, RepositoryFailResponse>> UpdateAsync( T entity, CancellationToken cancellationToken = default );
	}
}
