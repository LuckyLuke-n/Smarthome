using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Communication.Mqtt.Configuration;
using LSoftware.Communication.Mqtt.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LSoftware.Communication.Mqtt
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
