using InfluxDB.Client;
using LSoftware.Communication.Extensions;
using LSoftware.Metrics.Extensions;
using LSoftware.Metrics.Infux.Configuration;
using MongoDB.Driver;
using Smarthome.Api.Repositories;
using Smarthome.Api.Repositories.Devices.Mongo;

namespace Smarthome.Api
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMyServices( this IServiceCollection services, IConfiguration configuration )
		{
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
			services.AddMqttCommunication( configuration );
			services.AddMetricsLogging( configuration );


			return services;
		}
	}
}
