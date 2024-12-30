using System.Net;

namespace Smarthome.Api.Repositories.Devices
{
	public class DeviceRepositoryFailResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Message { get; set; }

		public DeviceRepositoryFailResponse()
		{
			StatusCode = HttpStatusCode.InternalServerError;
			Message = string.Empty;
		}

		public DeviceRepositoryFailResponse( HttpStatusCode statusCode, string error )
		{
			StatusCode = statusCode;
			Message = error;		
		}
	}
}
