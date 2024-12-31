using LSoftware.Communication.Mqtt;
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

			services.AddRepositoryServices( configuration );
			services.AddMqttCommunication( configuration );


			return services;
		}
	}
}
