using System.Net;

namespace Smarthome.Api.Repositories.Devices
{
	public class DeviceRepositoryFailResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Error { get; set; }

		public DeviceRepositoryFailResponse()
		{
			StatusCode = HttpStatusCode.InternalServerError;
			Error = string.Empty;
		}

		public DeviceRepositoryFailResponse( HttpStatusCode statusCode, string error )
		{
			StatusCode = statusCode;
			Error = error;		
		}
	}
}
