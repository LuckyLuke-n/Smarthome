using Microsoft.AspNetCore.Mvc;
using Smarthome.Core.Models;

namespace Smarthome.Api.Controllers
{
	[Route( "api/[controller]" )]
	[ApiController]
	public class DevicesController : ControllerBase
	{
		private ILogger<DevicesController> Logger { get; }

		public DevicesController( ILogger<DevicesController> logger )
		{
			Logger = logger;
		}

		[HttpPost()]
		[AutoValidateAntiforgeryToken]
		[ProducesResponseType( StatusCodes.Status201Created )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status409Conflict )]
		public async Task<IActionResult> AddDevice( [FromBody] CreateDeviceRequestDto dto, CancellationToken cancellationToken )
		{
			await Task.CompletedTask.ConfigureAwait( false );
			return Ok();
		}

		[HttpGet( "{id}" )]
		[AutoValidateAntiforgeryToken]
		[ProducesResponseType( typeof( GetDeviceResponseDto ), StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> GetDevice( [FromRoute] int id, CancellationToken cancellationToken )
		{
			await Task.CompletedTask.ConfigureAwait( false );
			return Ok();
		}

		[HttpGet()]
		[AutoValidateAntiforgeryToken]
		[ProducesResponseType( typeof( IEnumerable<GetDeviceResponseDto> ), StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		public async Task<IActionResult> GetDevices( CancellationToken cancellationToken )
		{
			await Task.CompletedTask.ConfigureAwait( false );
			return Ok();
		}

		[HttpPut( "{id}" )]
		[AutoValidateAntiforgeryToken]
		[ProducesResponseType( StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> UpdateDevice( [FromRoute] int id, [FromRoute] CreateDeviceRequestDto dto, CancellationToken cancellationToken )
		{
			await Task.CompletedTask.ConfigureAwait( false );
			return Ok();
		}

		[HttpDelete( "{id}" )]
		[AutoValidateAntiforgeryToken]
		[ProducesResponseType( StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> DeleteDevice( [FromRoute] int id, CancellationToken cancellationToken )
		{
			await Task.CompletedTask.ConfigureAwait( false );
			return Ok();
		}
	}
}
