namespace Smarthome.Api.Repositories.WeatherReport
{
	public interface IWeatherRepository
	{
		Task<RepositoryResponse<WeatherReport, WeatherRepositoryFailResponse>> GetWeatherDataAsync( CancellationToken cancellationToken = default );
	}
}
