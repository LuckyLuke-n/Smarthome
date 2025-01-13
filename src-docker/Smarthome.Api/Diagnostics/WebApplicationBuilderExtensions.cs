using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace Smarthome.Api.Diagnostics
{
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder AddOpenTelemetry( this WebApplicationBuilder builder )
		{
			const string serviceName = "Smarthome.Api";

			builder.Services.AddOpenTelemetry()
				.ConfigureResource( resource =>
				{
					resource
						.AddService( serviceName, serviceVersion: Assembly.GetExecutingAssembly().GetName().Version!.ToString(), serviceNamespace: "Smarthome" )
						.AddAttributes(
						[
							new KeyValuePair<string, object>("service.hostname", Environment.MachineName )
						] );
				} )				
				.WithTracing( tracing =>
					tracing
						.AddAspNetCoreInstrumentation()
						.AddHttpClientInstrumentation()
						.AddConsoleExporter()
						.AddOtlpExporter( options => 
							options.Endpoint = new Uri( Environment.GetEnvironmentVariable("SMARTHOME_Diagnostics__JaegerUrl")! ) )
						);

			return builder;
		}
	}
}
