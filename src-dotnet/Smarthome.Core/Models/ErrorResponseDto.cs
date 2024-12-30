using System.Net;

namespace Smarthome.Core.Models
{
	public class ErrorResponseDto
	{
		public HttpStatusCode OriginalStatusCode { get; set; }
		public string Code { get; set; }
		public string Message { get; set; }

		public ErrorResponseDto( HttpStatusCode originalStatusCode, string code, string message )
		{
			OriginalStatusCode = originalStatusCode;
			Code = code;
			Message = message;
		}

		public ErrorResponseDto( HttpStatusCode originalStatusCode, string code ) : this( originalStatusCode, code, "" )
		{ }
	}
}
