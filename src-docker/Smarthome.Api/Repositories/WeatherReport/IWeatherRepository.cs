namespace Smarthome.Api.Repositories.WeatherReport
{
	public interface IWeatherRepository
	{
		Task<WeatherRepositoryResponse<WeatherReport, WeatherRepositoryFailResponse>> GetWeatherDataAsync( CancellationToken cancellationToken = default );
	}
}
