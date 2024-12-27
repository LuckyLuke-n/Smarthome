using System.Net;

namespace Smarthome.Core.Models
{
	public class ErrorResponseDto
	{
		public HttpStatusCode OriginalStatusCode { get; set; }
		public string Code { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
	}
}
