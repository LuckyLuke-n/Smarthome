using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Smarthome.Api.Repositories.WeatherReport.Api
{
	public class TomorrowIoApiClient : IWeatherRepository
	{
		private WeatherApiConfiguration WeatherApiConfiguration { get; }
		private IHttpClientFactory HttpClientFactory { get; }
		private ILogger<TomorrowIoApiClient> Logger { get; }

		public TomorrowIoApiClient( IOptions<WeatherApiConfiguration> weatherApiOptions, IHttpClientFactory httpClientFactory, ILogger<TomorrowIoApiClient> logger )
		{
			WeatherApiConfiguration = weatherApiOptions.Value;
			HttpClientFactory = httpClientFactory;
			Logger = logger;

			if ( string.IsNullOrEmpty( WeatherApiConfiguration.Endpoint ) )
				Logger.LogWarning( "Weather api confguration url not set." );
		}

		public async Task<WeatherRepositoryResponse<WeatherReport, WeatherRepositoryFailResponse>> GetWeatherDataAsync( Core.DomainObjects.Location location, CancellationToken cancellationToken )
		{
			var client = HttpClientFactory.CreateClient();
			client.BaseAddress = new Uri( WeatherApiConfiguration.Endpoint );

			var response = await client.GetAsync( $"?location={location.City}&apikey={WeatherApiConfiguration.ApiKey}", cancellationToken ).ConfigureAwait( false );

			if ( response is null || !response.IsSuccessStatusCode )
			{
				string errorContent;
				if ( response is null )
					errorContent = "unknown";
				else
					errorContent = await response.Content.ReadAsStringAsync( cancellationToken ).ConfigureAwait( false );

				return WeatherRepositoryResponse<WeatherReport, WeatherRepositoryFailResponse>.CreateFail( new() { StatusCode = response?.StatusCode ?? HttpStatusCode.InternalServerError, Message = errorContent } );
			}

			var content = await response.Content.ReadAsStringAsync( cancellationToken ).ConfigureAwait( false );

			TomorrowIoWeatherDto? dto = null;
			try
			{
				JsonSerializerOptions options = new()
				{
					PropertyNameCaseInsensitive = true,
					UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
				};
				dto = JsonSerializer.Deserialize<TomorrowIoWeatherDto>( content, options );
			}
			catch ( JsonException ex )
			{
				Logger.LogWarning( ex, "Could no deserialize result from tomorrow.io api." );
			}

			if ( dto is null )
				return WeatherRepositoryResponse<WeatherReport, WeatherRepositoryFailResponse>.CreateFail( new() { StatusCode = HttpStatusCode.InternalServerError, Message = "Could no deserialize result from tomorrow.io api." } );

			var weatherReport = dto.ToWeatherReport();

			return WeatherRepositoryResponse<WeatherReport, WeatherRepositoryFailResponse>.CreateSuccess( weatherReport );
		}
	}
}
