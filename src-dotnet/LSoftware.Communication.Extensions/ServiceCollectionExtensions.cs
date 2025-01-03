using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Communication.Mqtt.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LSoftware.Communication.Mqtt.Handler;

namespace LSoftware.Communication.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMqttCommunication( this IServiceCollection services, IConfiguration configuration )
		{

			services.Configure<MqttConfiguration>( configuration.GetSection( MqttConfiguration.Section ) );

			services.AddTransient<MqttClientHandler>();
			services.AddTransient<Func<MqttClientHandler>>( serviceProvider =>
				() => serviceProvider.GetService<MqttClientHandler>() );

			services.AddSingleton<IConnectionHandler, MqttConnectionHandler>();

			return services;
		}
	}
}
