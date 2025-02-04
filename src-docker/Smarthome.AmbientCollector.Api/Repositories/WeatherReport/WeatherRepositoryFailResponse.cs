using System.Net;

namespace Smarthome.AmbientCollector.Api.Repositories.WeatherReport
{
	public class WeatherRepositoryFailResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Message { get; set; }

		public WeatherRepositoryFailResponse()
		{
			StatusCode = HttpStatusCode.InternalServerError;
			Message = string.Empty;
		}

		public WeatherRepositoryFailResponse( HttpStatusCode statusCode, string error )
		{
			StatusCode = statusCode;
			Message = error;
		}
	}
}
