using System.Net;
using Microsoft.AspNetCore.Mvc;
using Smarthome.AmbientCollector.Api.Extensions;
using Smarthome.AmbientCollector.Api.Repositories.Locations;
using Smarthome.Core.Models;

namespace Smarthome.AmbientCollector.Api.Controllers
{
	[Route( "api/[controller]" )]
	[ApiController]
	public class LocationsController : ControllerBase
	{
		private ILogger<LocationsController> Logger { get; }
		private ILocationRepository LocationRepository { get; }

		public LocationsController( ILogger<LocationsController> logger, ILocationRepository locationRepository  )
		{
			Logger = logger;
			LocationRepository = locationRepository;
		}

		[HttpPost()]
		[ProducesResponseType( StatusCodes.Status201Created )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status409Conflict )]
		public async Task<IActionResult> AddLocation( [FromBody] CreateLocationRequestDto dto, CancellationToken cancellationToken )
		{
			var location = dto.ToEntity();
			var response = await LocationRepository.CreateAsync( location, cancellationToken );

			if ( response.IsSuccess )
				return CreatedAtAction( nameof( GetLocation ), new { id = response.ValueSuccess!.Id }, response.ValueSuccess );
			else
			{
				var failedResponse = response.ValueFail;
				switch ( failedResponse.StatusCode )
				{
					case HttpStatusCode.Conflict:
						return Conflict( new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
					case HttpStatusCode.BadRequest:
						return BadRequest( new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
					default:
						return StatusCode( StatusCodes.Status500InternalServerError, new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
				}
			}
		}

		[HttpGet( "{id}" )]
		[ProducesResponseType( typeof( GetLocationResponseDto ), StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> GetLocation( [FromRoute] Guid id, CancellationToken cancellationToken )
		{
			var response = await LocationRepository.ReadAsync( id, cancellationToken );

			if ( response.IsSuccess )
				return Ok( response.ValueSuccess );
			else
			{
				var failedResponse = response.ValueFail;
				switch ( failedResponse.StatusCode )
				{
					case HttpStatusCode.NotFound:
						return NotFound( new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
					default:
						return StatusCode( StatusCodes.Status500InternalServerError, new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
				}
			}
		}

		[HttpGet()]
		[ProducesResponseType( typeof( IEnumerable<GetLocationResponseDto> ), StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		public async Task<IActionResult> GetLocations( CancellationToken cancellationToken )
		{
			var response = await LocationRepository.ReadAllAsync( cancellationToken );

			if ( response.IsSuccess )
				return Ok( response.ValueSuccess );
			else
			{
				var failedResponse = response.ValueFail;
				switch ( failedResponse.StatusCode )
				{
					case HttpStatusCode.BadRequest:
						return BadRequest( new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
					default:
						return StatusCode( StatusCodes.Status500InternalServerError, new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
				}
			}
		}

		[HttpPut( "{id}" )]
		[ProducesResponseType( StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> UpdateLocation( [FromRoute] Guid id, [FromBody] UpdateLocationRequestDto dto, CancellationToken cancellationToken )
		{
			var location = dto.ToEntity();
			location.Id = id.ToString();

			var response = await LocationRepository.UpdateAsync( location, cancellationToken );

			if ( response.IsSuccess )
				return Ok();
			else
			{
				var failedResponse = response.ValueFail;
				switch ( failedResponse.StatusCode )
				{
					case HttpStatusCode.NotFound:
						return NotFound( new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
					default:
						return StatusCode( StatusCodes.Status500InternalServerError, new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
				}
			}
		}

		[HttpDelete( "{id}" )]
		[ProducesResponseType( StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> DeleteLocation( [FromRoute] Guid id, CancellationToken cancellationToken )
		{
			var response = await LocationRepository.DeleteAsync( id, cancellationToken );

			if ( response.IsSuccess )
				return Ok();
			else
			{
				var failedResponse = response.ValueFail;
				switch ( failedResponse.StatusCode )
				{
					case HttpStatusCode.NotFound:
						return NotFound( new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
					default:
						return StatusCode( StatusCodes.Status500InternalServerError, new ErrorResponseDto( failedResponse.StatusCode, failedResponse.StatusCode.ToString(), failedResponse.Message ) );
				}
			}
		}
	}
}
