using MongoDB.Driver;
using Smarthome.Api.Repositories;

namespace Smarthome.Api
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMyServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.AddSingleton<IMongoClient, MongoClient>( sp =>
			{
				var settings = MongoClientSettings.FromConnectionString( Environment.GetEnvironmentVariable( "SMARTHOME_MongoDb_ConnectionString" ) );
				return new MongoClient( settings );
			} );

			services.AddRepositoryServices( configuration );


			return services;
		}
	}
}
