using LSoftware.Repository.MongoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LSoftware.Repository.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMongoDbRepositoryServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.Configure<MongoDbConfiguration>( configuration.GetSection( MongoDbConfiguration.Section ) );

			return services;
		}
	}
}
