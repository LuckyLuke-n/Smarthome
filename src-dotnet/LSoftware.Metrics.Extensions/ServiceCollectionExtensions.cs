using LSoftware.Metrics.Abstractions;
using LSoftware.Metrics.Influx;
using LSoftware.Metrics.Influx.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LSoftware.Metrics.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMetricsLogging( this IServiceCollection services, IConfiguration configuration )
		{

			services.Configure<InfluxDbConfiguration>( configuration.GetSection( InfluxDbConfiguration.Section ) );
			services.AddSingleton( typeof( IMetricsLogger<> ), typeof( InfluxMetricsLogger<> ) );

			return services;
		}
	}
}
