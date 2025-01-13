
using LSoftware.Communication.Mqtt.Configuration;
using LSoftware.Metrics.Abstractions;
using Microsoft.Extensions.Options;
using Smarthome.Api.Configuration;
using Smarthome.Api.Monitoring.Health.Dtos;
using Smarthome.Api.Monitoring.WeatherData;
using Smarthome.Api.Repositories.WeatherReport.Api;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static NodaTime.TimeZones.ZoneEqualityComparer;

namespace Smarthome.Api.Monitoring.Health
{
	public class HealthMonitor : IHostedService
	{
		private ApiConfiguration ApiConfiguration { get; }
		private MqttConfiguration MqttConfiguration { get; }

		private IHttpClientFactory HttpClientFactory { get; }
		private IMetricsLogger<ApiHealthData> WeatherLogger { get; }
		private ILogger<HealthMonitor> Logger { get; }
		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private Timer HealthTimer { get; }

		public HealthMonitor( IOptions<ApiConfiguration> apiConfigOptions,
			IOptions<MqttConfiguration> mqttConfigOptions,
			IHttpClientFactory httpClientFactory,
			IMetricsLogger<ApiHealthData> weatherLogger,
			ILogger<HealthMonitor> logger )
		{
			ApiConfiguration = apiConfigOptions.Value;
			MqttConfiguration = mqttConfigOptions.Value;
			HttpClientFactory = httpClientFactory;
			WeatherLogger = weatherLogger;
			Logger = logger;
			HealthTimer = new( TriggerWeatherTimerActionsAsync, null, int.MaxValue, int.MaxValue );
		}

		private async void TriggerWeatherTimerActionsAsync( object? state )
		{
			JsonSerializerOptions options = new()
			{
				PropertyNameCaseInsensitive = true,
				UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
			};

			ApiHealthData status = new();

			foreach ( var item in ApiConfiguration.HealthEndpoints )
			{
				if ( CancellationTokenSource.IsCancellationRequested )
					return;

				var client = HttpClientFactory.CreateClient( item.Key.ToString() );

				try
				{
					if (item.Key == ServiceType.RabbitMq )
					{
						var byteArray = Encoding.ASCII.GetBytes( $"{MqttConfiguration.Username}:{MqttConfiguration.Password}" );
						client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Basic", Convert.ToBase64String( byteArray ) );
					}

					var result = await client.GetAsync( item.Value ).ConfigureAwait( false );
					var content = await result.Content.ReadAsStringAsync().ConfigureAwait( false );

					switch ( item.Key )
					{
						case ServiceType.RabbitMq:
							var rabbit = JsonSerializer.Deserialize<RabbitMqHealthDto>( content, options );
							status.RabbitMq = rabbit?.Status ?? "unknown";
							break;
						case ServiceType.Influx:
							var influx = JsonSerializer.Deserialize<InfluxDbHealthDto>( content, options );
							status.InfluxDb = influx?.Status ?? "unknown";
							break;
						case ServiceType.Grafana:
							var grafana = JsonSerializer.Deserialize<GrafanaHealthDtocs>( content, options );
							status.Grafana = grafana?.Database ?? "unknown";
							break;
						default:
							break;
					}
				}
				catch ( Exception ex )
				{
					Logger.LogWarning( ex, "Cannot get health status for {Service}", item.Key.ToString() );
				}
			}

			WeatherLogger.SendInstant( status );
		}

		public async Task StartAsync( CancellationToken cancellationToken )
		{
			TriggerWeatherTimerActionsAsync( null );
			HealthTimer.Change( TimeSpan.FromSeconds( 1 ), TimeSpan.FromMinutes( 60 ) );
			await Task.CompletedTask.ConfigureAwait( false );
		}

		public async Task StopAsync( CancellationToken cancellationToken )
		{
			await CancellationTokenSource.CancelAsync().ConfigureAwait( false );
			await HealthTimer.DisposeAsync().ConfigureAwait( false );
		}
	}
}
