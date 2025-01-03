using Smarthome.Api.Repositories.Devices;
using Smarthome.Api.Repositories.Devices.Mongo;
using Smarthome.Api.Repositories.WeatherReport;
using Smarthome.Api.Repositories.WeatherReport.Api;

namespace Smarthome.Api.Repositories
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRepositoryServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.Configure<MongoDbConfiguration>( configuration.GetSection( MongoDbConfiguration.Section ) );
			services.AddSingleton<IDeviceRepository, DeviceMongoRepository>();

			return services;
		}

		public static IServiceCollection AddWeatherRepositoryServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.Configure<WeatherApiConfiguration>( configuration.GetSection( WeatherApiConfiguration.Section ) );
			services.AddHttpClient();
			services.AddSingleton<IWeatherRepository, TomorrowIoApiClient>();

			return services;
		}
	}
}
