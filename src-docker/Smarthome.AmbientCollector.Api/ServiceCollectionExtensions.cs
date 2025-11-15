using System;
using LSoftware.Communication.Extensions;
using LSoftware.Repository.MongoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Smarthome.AmbientCollector.Api.Configuration;
using Smarthome.AmbientCollector.Api.Monitoring.MessageBus;
using Smarthome.AmbientCollector.Api.Monitoring.WeatherData;
using Smarthome.AmbientCollector.Api.Repositories;

namespace Smarthome.AmbientCollector.Api
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMyServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.Configure<ApiConfiguration>( configuration.GetSection( ApiConfiguration.Section ) );
			
			services.AddWeatherRepositoryServices( configuration );
			services.AddMqttCommunication( configuration );

			services.AddHostedService<DeviceMonitor>();
			services.AddHostedService<WeatherMonitor>();

			return services;
		}
	}
}
