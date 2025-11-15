using Smarthome.AmbientCollector.Api.Repositories.WeatherReport;
using Smarthome.AmbientCollector.Api.Repositories.WeatherReport.Api;

namespace Smarthome.AmbientCollector.Api.Repositories
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddWeatherRepositoryServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.Configure<WeatherApiConfiguration>( configuration.GetSection( WeatherApiConfiguration.Section ) );
			services.AddHttpClient();
			services.AddSingleton<IWeatherRepository, TomorrowIoApiClient>();

			return services;
		}
	}
}
