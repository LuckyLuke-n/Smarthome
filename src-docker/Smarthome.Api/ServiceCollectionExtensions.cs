using Smarthome.Api.Repositories;

namespace Smarthome.Api
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMyServices( this IServiceCollection services, IConfiguration configuration )
		{
			services.AddRepositoryServices( configuration );


			return services;
		}
	}
}
