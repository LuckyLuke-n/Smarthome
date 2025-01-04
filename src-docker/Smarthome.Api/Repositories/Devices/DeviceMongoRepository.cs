using LSoftware.Repository.Abstractions;
using LSoftware.Repository.MongoDb;
using MongoDB.Driver;
using Smarthome.Core.DomainObjects;
using System.Net;

namespace Smarthome.Api.Repositories.Devices
{
	public class DeviceMongoRepository : MongoDbRepository<Device>, IDeviceRepository
	{
		public DeviceMongoRepository( IMongoClient mongoClient, ILogger<MongoDbRepository<Device>> logger ) : base( mongoClient, logger )
		{
		}

		public async Task<RepositoryResponse<IEnumerable<Device>, RepositoryFailResponse>> ReadReadyAndSendingAsync( CancellationToken cancellationToken = default )
		{
			var filter = Builders<Device>.Filter
				.In( d => d.State, [ State.Ready, State.Sending ] );

			return await ReadMultipleAsync( filter, cancellationToken ).ConfigureAwait( false );
		}

		public async Task<RepositoryResponse<Device, RepositoryFailResponse>> ReadAsync( string hostname, HardwareType type, CancellationToken cancellationToken = default )
		{
			if ( Collection is null )
				return RepositoryResponse<Device, RepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "No connection to the mongo collection." } );

			var filter = Builders<Device>.Filter.And(
				Builders<Device>.Filter.Eq( d => d.Hostname, hostname.ToLower() ),
				Builders<Device>.Filter.Eq( d => d.Hardware.Type, type ) );

			try
			{
				var cursor = await Collection.FindAsync<Device>( filter, null, cancellationToken ).ConfigureAwait( false );
				var devices = await cursor.ToListAsync( cancellationToken ).ConfigureAwait( false );

				if ( devices.Count == 0 )
				{
					RepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be delted. Document not found." };
					return RepositoryResponse<Device, RepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<Device, RepositoryFailResponse>.CreateSuccess( devices[ 0 ] );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				RepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<Device, RepositoryFailResponse>.CreateFail( fail );
			}
		}

		public override async Task<RepositoryResponse<Device, RepositoryFailResponse>> UpdateAsync( Device device, CancellationToken cancellationToken = default )
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
					RepositoryFailResponse fail = new() { StatusCode = HttpStatusCode.NotFound, Message = "Document cannot be updated. Document not found." };
					return RepositoryResponse<Device, RepositoryFailResponse>.CreateFail( fail );
				}

				return RepositoryResponse<Device, RepositoryFailResponse>.CreateSuccess( updated );
			}
			catch ( Exception ex )
			{
				Logger.LogCritical( ex, "Error reading from mongo." );
				RepositoryFailResponse fail = new()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Message = ex.Message,
				};
				return RepositoryResponse<Device, RepositoryFailResponse>.CreateFail( fail );
			}
		}
	}
}
