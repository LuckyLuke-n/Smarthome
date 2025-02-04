using InfluxDB.Client;
using LSoftware.Communication.Extensions;
using LSoftware.Metrics.Extensions;
using LSoftware.Metrics.InfluxDb.Configuration;
using LSoftware.Repository.MongoDb;
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

			services.AddSingleton<IMongoClient, MongoClient>( sp =>
			{
				var settings = MongoClientSettings.FromConnectionString( Environment.GetEnvironmentVariable( MongoDbConfiguration.ConnectionStringEnvVar ) );
				return new MongoClient( settings );
			} );

			services.AddSingleton<InfluxDBClient>( sp =>
			{
				var url = Environment.GetEnvironmentVariable( InfluxDbConfiguration.UrlEnvVar );
				var token = Environment.GetEnvironmentVariable( InfluxDbConfiguration.TokenEnvVar );

				return new( url, token );
			} );

			services.AddRepositoryServices( configuration );
			services.AddWeatherRepositoryServices( configuration );
			services.AddMqttCommunication( configuration );
			services.AddMetricsLogging( configuration );

			services.AddHostedService<DeviceMonitor>();
			services.AddHostedService<WeatherMonitor>();

			return services;
		}
	}
}
