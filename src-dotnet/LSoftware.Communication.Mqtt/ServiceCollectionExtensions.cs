using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Communication.Mqtt.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LSoftware.Communication.Mqtt
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMqttCommunication( this IServiceCollection services, IConfiguration configuration )
		{
			services.AddTransient<MqttClientHandler>( serviceProvider =>
			{
				var connectionString = configuration.GetConnectionString( "smarthome-mqtt" ) ?? string.Empty;
				return ActivatorUtilities.CreateInstance<MqttClientHandler>( serviceProvider, connectionString );
			} );
			
			
			services.AddTransient<Func<MqttClientHandler>>( serviceProvider =>
				() => serviceProvider.GetService<MqttClientHandler>()! );

			services.AddSingleton<IConnectionHandler, MqttConnectionHandler>();

			return services;
		}
	}
}
