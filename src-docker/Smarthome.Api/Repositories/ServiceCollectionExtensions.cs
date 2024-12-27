using Smarthome.Api.Repositories.Devices;
using Smarthome.Api.Repositories.Devices.Mongo;

namespace Smarthome.Api.Repositories
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRepositoryServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.Configure<MongoDbConfiguration>( configuration.GetSection( MongoDbConfiguration.Prefix ) );

			// services.AddTransient<IMyService, MyService>();
			services.AddScoped<IDeviceRepository, DeviceMongoRepository>();

			return services;
		}
	}
}
