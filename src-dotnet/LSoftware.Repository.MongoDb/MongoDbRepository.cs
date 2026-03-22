using LSoftware.Repository.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Net;

namespace LSoftware.Repository.MongoDb
{
	public abstract class MongoDbRepository<T> : IGenericRepository<T> where T : class, IEntity
	{
		protected IMongoCollection<T>? Collection { get; set; }
		protected ILogger<MongoDbRepository<T>> Logger { get; }

		protected MongoDbRepository( IMongoClient mongoClient, ILogger<MongoDbRepository<T>> logger )
		{
			Logger = logger;

			try
			{
				var database = mongoClient.GetDatabase( "Smarthome" );
				Collection = database.GetCollection<T>( typeof(T).Name );
			}
			catch ( MongoException ex )
			{
				Logger.LogCritical( ex, "Mongo initialization failed." );
			}
		}

		protected RepositoryResponse<T, RepositoryFailResponse> NotConnectedFailedResponse()
		{
			return RepositoryResponse<T, RepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );
		}

		public async Task<RepositoryResponse<T, RepositoryFailResponse>> CreateAsync( T entity, CancellationToken cancellationToken = default )
		{
			entity.SetAsNew();

			if ( Collection is null )
				return NotConnectedFailedResponse();

			try
			{
				await Collection.InsertOneAsync( entity, null, cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error writing to mongo." );
				RepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<T,RepositoryFailResponse>.CreateFail( fail );
			}

			return RepositoryResponse<T, RepositoryFailResponse>.CreateSuccess( entity );
		}

		public async Task<RepositoryResponse<RepositoryFailResponse>> DeleteAsync( Guid id, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<RepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			var filter = Builders<T>.Filter
				.Eq( e => e.Id, id.ToString() );

			try
			{
				var device = await Collection.FindOneAndDeleteAsync<T>( filter, null, cancellationToken ).ConfigureAwait( false );

				if ( device is null )
				{
					RepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be delted. Document not found." };
					return RepositoryResponse<RepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<RepositoryFailResponse>.CreateSuccess();
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				RepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<RepositoryFailResponse>.CreateFail( fail );
			}
		}

		public async Task<RepositoryResponse<IEnumerable<T>, RepositoryFailResponse>> ReadAllAsync( CancellationToken cancellationToken = default ) => await ReadMultipleAsync( FilterDefinition<T>.Empty, cancellationToken ).ConfigureAwait( false );

		public async Task<RepositoryResponse<T, RepositoryFailResponse>> ReadAsync( Guid id, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<T, RepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			var filter = Builders<T>.Filter
				.Eq( d => d.Id, id.ToString() );

			try
			{
				var cursor = await Collection.FindAsync<T>( filter, null, cancellationToken ).ConfigureAwait( false );
				var devices = await cursor.ToListAsync( cancellationToken ).ConfigureAwait( false );

				if ( devices.Count == 0 )
				{
					RepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be read. Document not found." };
					return RepositoryResponse<T, RepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<T, RepositoryFailResponse>.CreateSuccess( devices[ 0 ] );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				RepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<T, RepositoryFailResponse>.CreateFail( fail );
			}
		}

		public abstract Task<RepositoryResponse<T, RepositoryFailResponse>> UpdateAsync( T entity, CancellationToken cancellationToken = default );

		protected async Task<RepositoryResponse<IEnumerable<T>, RepositoryFailResponse>> ReadMultipleAsync( FilterDefinition<T> filter, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<IEnumerable<T>, RepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			try
			{
				var cursor = await Collection.FindAsync<T>( filter, null, cancellationToken ).ConfigureAwait( false );
				var entities = await cursor.ToListAsync( cancellationToken ).ConfigureAwait( false );
				return RepositoryResponse<IEnumerable<T>, RepositoryFailResponse>.CreateSuccess( entities );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				RepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<IEnumerable<T>, RepositoryFailResponse>.CreateFail( fail );
			}
		}
	}
}
