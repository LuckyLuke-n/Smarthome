using LSoftware.Repository.Abstractions;
using LSoftware.Repository.MongoDb;
using MongoDB.Driver;
using Smarthome.Core.DomainObjects;
using System.Net;

namespace Smarthome.Api.Repositories.Locations
{
	public class LocationMongoRepository : MongoDbRepository<Location>, ILocationRepository
	{
		public LocationMongoRepository( IMongoClient mongoClient, ILogger<MongoDbRepository<Location>> logger ) : base( mongoClient, logger )
		{
		}

		public async override Task<RepositoryResponse<Location, RepositoryFailResponse>> UpdateAsync( Location entity, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return NotConnectedFailedResponse();

			var filter = Builders<Location>.Filter
				.Eq( l => l.Id, entity.Id );

			var update = Builders<Location>.Update
				.Set( d => d.Latitude, entity.Latitude )
				.Set( d => d.Longitude, entity.Longitude )
				.Set( d => d.City, entity.City );

			try
			{
				var updated = await Collection.FindOneAndUpdateAsync<Location>( filter, update, null, cancellationToken ).ConfigureAwait( false );

				if ( updated is null )
				{
					RepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be updated. Document not found." };
					return RepositoryResponse<Location, RepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<Location, RepositoryFailResponse>.CreateSuccess( updated );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				RepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<Location, RepositoryFailResponse>.CreateFail( fail );
			}
		}
	}
}
