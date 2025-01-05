using Smarthome.Api.Repositories.Devices;
using Smarthome.Api.Repositories.WeatherReport;
using Smarthome.Api.Repositories.WeatherReport.Api;
using LSoftware.Repository.Extensions;
using Smarthome.Api.Repositories.Locations;

namespace Smarthome.Api.Repositories
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRepositoryServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.AddMongoDbRepositoryServices( configuration );
			services.AddSingleton<IDeviceRepository, DeviceMongoRepository>();
			services.AddSingleton<ILocationRepository, LocationMongoRepository>();

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
