using System.Reflection;
using LSoftware.Communication.Mqtt.Diagnostics.Meters;
using OpenTelemetry.Resources;
using Smarthome.AmbientCollector.Api.Diagnostics.Meters;

namespace Smarthome.AmbientCollector.Api.Diagnostics
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddAmbientCollectorOpenTelemetry( this IServiceCollection services )
		{
			services.AddOpenTelemetry()
				.ConfigureResource(resource =>
				{
					resource
						.AddService("AmbientCollector", "Smarthome.AmbientCollector",
							Assembly.GetExecutingAssembly().GetName().Version!.ToString())
						.AddAttributes(
						[

							new KeyValuePair<string, object>("service.hostname", Environment.MachineName)
						]);
				})
				.WithMetrics( meters =>
					meters.AddMeter( EnvironmentMeter.Name )
						.AddMeter( WeatherMeter.Name )
						.AddMeter( MqttBrokerMeter.Name )
					)
				.WithLogging()
				.WithTracing();

			return services;
		}
	}
}
