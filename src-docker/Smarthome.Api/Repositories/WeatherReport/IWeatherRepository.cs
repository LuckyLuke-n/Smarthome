using Smarthome.Core.DomainObjects;

namespace Smarthome.AmbientCollector.Api.Repositories.WeatherReport
{
	public interface IWeatherRepository
	{
		Task<WeatherRepositoryResponse<WeatherReport, WeatherRepositoryFailResponse>> GetWeatherDataAsync( Location location, CancellationToken cancellationToken = default );
	}
}
