using System.Net;

namespace LSoftware.Repository.Abstractions
{
	public class RepositoryFailResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Message { get; set; }

		public RepositoryFailResponse()
		{
			StatusCode = HttpStatusCode.InternalServerError;
			Message = string.Empty;
		}

		public RepositoryFailResponse( HttpStatusCode statusCode, string error )
		{
			StatusCode = statusCode;
			Message = error;
		}
	}
}
