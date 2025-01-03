using MongoDB.Driver;
using Smarthome.Core.DomainObjects;
using System.Net;

namespace Smarthome.Api.Repositories.Devices.Mongo
{
	public class DeviceMongoRepository : IDeviceRepository
	{
		private IMongoCollection<Device>? Collection { get; set; }
		private ILogger<DeviceMongoRepository> Logger { get; }

		public DeviceMongoRepository( IMongoClient mongoClient, ILogger<DeviceMongoRepository> logger )
		{
			Logger = logger;

			try
			{
				var database = mongoClient.GetDatabase( "Smarthome" );
				Collection = database.GetCollection<Device>( nameof( Device ) );
			}
			catch ( MongoException ex )
			{
				Logger.LogCritical( ex, "Mongo initialization failed." );
			}
		}

		private RepositoryResponse<Device, DeviceRepositoryFailResponse> NotConnectedFailedResponse()
		{
			return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );
		}

		public async Task<RepositoryResponse<Device, DeviceRepositoryFailResponse>> CreateAsync( Device device, CancellationToken cancellationToken = default )
		{
			device.SetAsNew();

			if ( Collection is null )
				return NotConnectedFailedResponse();

			try
			{
				await Collection.InsertOneAsync( device, null, cancellationToken ).ConfigureAwait( false );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error writing to mongo." );
				DeviceRepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( fail );
			}

			return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateSuccess( device );
		}

		public async Task<RepositoryResponse<IEnumerable<Device>, DeviceRepositoryFailResponse>> ReadAllAsync( CancellationToken cancellationToken = default ) => await ReadMultipleAsync( FilterDefinition<Device>.Empty, cancellationToken ).ConfigureAwait( false );

		public async Task<RepositoryResponse<IEnumerable<Device>, DeviceRepositoryFailResponse>> ReadReadyAndSendingAsync( CancellationToken cancellationToken = default )
		{
			var filter = Builders<Device>.Filter
				.In( d => d.State, [ State.Ready, State.Sending ] );

			return await ReadMultipleAsync( filter, cancellationToken ).ConfigureAwait( false );
		}

		public async Task<RepositoryResponse<DeviceRepositoryFailResponse>> DeleteAsync( Guid id, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<DeviceRepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			var filter = Builders<Device>.Filter
				.Eq( d => d.Id, id.ToString() );

			try
			{
				var device = await Collection.FindOneAndDeleteAsync<Device>( filter, null, cancellationToken ).ConfigureAwait( false );

				if ( device is not null )
				{
					DeviceRepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be delted. Document not found." };
					return RepositoryResponse<DeviceRepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<DeviceRepositoryFailResponse>.CreateSuccess();
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				DeviceRepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<DeviceRepositoryFailResponse>.CreateFail( fail );
			}
		}

		public async Task<RepositoryResponse<Device, DeviceRepositoryFailResponse>> ReadAsync( Guid id, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			var filter = Builders<Device>.Filter
				.Eq( d => d.Id, id.ToString() );

			try
			{
				var cursor = await Collection.FindAsync<Device>( filter, null, cancellationToken ).ConfigureAwait( false );
				var devices = await cursor.ToListAsync( cancellationToken ).ConfigureAwait( false );

				if ( devices.Count == 0 )
				{
					DeviceRepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be delted. Document not found." };
					return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateSuccess( devices[ 0 ] );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				DeviceRepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( fail );
			}
		}

		public async Task<RepositoryResponse<Device, DeviceRepositoryFailResponse>> ReadAsync( string hostname, HardwareType type, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			var filter = Builders<Device>.Filter.And(
				Builders<Device>.Filter.Eq( d => d.Hostname, hostname.ToLower() ),
				Builders<Device>.Filter.Eq( d => d.Hardware.Type, type ) );

			try
			{
				var cursor = await Collection.FindAsync<Device>( filter, null, cancellationToken ).ConfigureAwait( false );
				var devices = await cursor.ToListAsync( cancellationToken ).ConfigureAwait( false );

				if ( devices.Count == 0 )
				{
					DeviceRepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be delted. Document not found." };
					return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateSuccess( devices[ 0 ] );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				DeviceRepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( fail );
			}
		}

		public async Task<RepositoryResponse<Device, DeviceRepositoryFailResponse>> UpdateAsync( Device device, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return NotConnectedFailedResponse();

			var filter = Builders<Device>.Filter
				.Eq( d => d.Id, device.Id );

			var update = Builders<Device>.Update
				.Set( d => d.Hostname, device.Hostname )
				.Set( d => d.Hardware, device.Hardware )
				.Set( d => d.Location, device.Location )
				.Set( d => d.State, device.State );

			try
			{
				var updated = await Collection.FindOneAndUpdateAsync<Device>( filter, update, null, cancellationToken ).ConfigureAwait( false );

				if ( updated is null )
				{
					DeviceRepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be updated. Document not found." };
					return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateSuccess( updated );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				DeviceRepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<Device, DeviceRepositoryFailResponse>.CreateFail( fail );
			}
		}

		private async Task<RepositoryResponse<IEnumerable<Device>, DeviceRepositoryFailResponse>> ReadMultipleAsync( FilterDefinition<Device> filter, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<IEnumerable<Device>, DeviceRepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			try
			{
				var cursor = await Collection.FindAsync<Device>( filter, null, cancellationToken ).ConfigureAwait( false );
				var devices = await cursor.ToListAsync( cancellationToken ).ConfigureAwait( false );
				return RepositoryResponse<IEnumerable<Device>, DeviceRepositoryFailResponse>.CreateSuccess( devices );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				DeviceRepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<IEnumerable<Device>, DeviceRepositoryFailResponse>.CreateFail( fail );
			}
		}
	}
}
