using OpenTelemetry.Metrics;
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
				.WithMetrics( metrics => metrics
					.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation()
					// metrics provided by ASP.NET
					.AddMeter("Microsoft.AspNetCore.Hosting")
					.AddMeter("Microsoft.AspNetCore.Server.Kestrel")

					.AddOtlpExporter( options =>
						options.Endpoint = new Uri( Environment.GetEnvironmentVariable( DiagnosticsConfiguration.OtlpEndpointEnvVar )! ) )
					//.AddInfluxDBMetricsExporter( options =>
					//{
					//	options.Endpoint = new( Environment.GetEnvironmentVariable( InfluxDbConfiguration.UrlEnvVar )! );
					//	options.Token = Environment.GetEnvironmentVariable( InfluxDbConfiguration.TokenEnvVar )!;
					//	options.MetricExportIntervalMilliseconds = 20000;
					//} )
				);

			if ( bool.TryParse( Environment.GetEnvironmentVariable( DiagnosticsConfiguration.TracingEnabledEnvVar ), out bool enabled ) && enabled )
			{
				builder.Services.AddOpenTelemetry()
				.WithTracing( tracing =>
					tracing
						.AddAspNetCoreInstrumentation()
						.AddHttpClientInstrumentation()
						.AddRabbitMQInstrumentation()
						.AddConsoleExporter()
						
						.AddOtlpExporter( options =>
							options.Endpoint = new Uri( Environment.GetEnvironmentVariable( DiagnosticsConfiguration.OtlpEndpointEnvVar )! ) )
						);
			}

			return builder;
		}
	}
}
