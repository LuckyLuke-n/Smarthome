using Microsoft.AspNetCore.Mvc;

namespace Smarthome.AmbientCollector.Api.Controllers
{
	[Route( "api/[controller]" )]
	[ApiController]
	public class StatusController : ControllerBase
	{
		[HttpGet( "version" )]
		[ProducesResponseType( StatusCodes.Status200OK )]
		public IActionResult GetVersion()
		{
			return Ok( ServiceConstants.Version );
		}
	}
}
