using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Smarthome.Core.DomainObjects;

namespace Smarthome.Api.Repositories.Devices.Mongo
{
	public class DeviceMongoRepository : IDeviceRepository
	{
		private IMongoClient? Client { get; set; }
		private ILogger Logger { get; }

		public DeviceMongoRepository( IOptions<MongoDbConfiguration> options, ILogger logger )
		{
			Logger = logger;

			try
			{
				Client = new MongoClient( options.Value.ConnectionString );
			}
			catch ( MongoException ex )
			{
				Logger.LogCritical( ex, "Mongo client cannot be created." );
			}
		}

		public Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> CreateAsync( Device device, CancellationToken cancellationToken = default )
		{
			throw new NotImplementedException();
		}

		public Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> DeleteAsync( int id, CancellationToken cancellationToken = default )
		{
			throw new NotImplementedException();
		}

		public Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> ReadAsync( int id, CancellationToken cancellationToken = default )
		{
			throw new NotImplementedException();
		}

		public Task<RepositoryResponse<DeviceRepositorySuccessResponse, DeviceRepositoryFailResponse>> UpdateAsync( Device device, CancellationToken cancellationToken = default )
		{
			throw new NotImplementedException();
		}
	}
}
