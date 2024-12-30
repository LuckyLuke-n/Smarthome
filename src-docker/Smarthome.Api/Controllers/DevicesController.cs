using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smarthome.Api.Repositories.Devices;
using Smarthome.Core.DomainObjects;
using Smarthome.Core.Models;
using System.Net;

namespace Smarthome.Api.Controllers
{
	[Route( "api/[controller]" )]
	[ApiController]
	public class DevicesController : ControllerBase
	{
		private IMapper Mapper { get; }
		private ILogger<DevicesController> Logger { get; }
		private IDeviceRepository DeviceRepository { get; }

		public DevicesController( IMapper mapper, ILogger<DevicesController> logger, IDeviceRepository deviceRepository  )
		{
			Mapper = mapper;
			Logger = logger;
			DeviceRepository = deviceRepository;
		}

		[HttpPost()]
		//[AutoValidateAntiforgeryToken]
		[ProducesResponseType( StatusCodes.Status201Created )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status409Conflict )]
		public async Task<IActionResult> AddDevice( [FromBody] CreateDeviceRequestDto dto, CancellationToken cancellationToken )
		{
			var device = Mapper.Map<Device>( dto );
			var response = await DeviceRepository.CreateAsync( device, cancellationToken );

			if ( response.IsSuccess )
				return CreatedAtAction( nameof( GetDevice ), new { id = response.ValueSuccess!.Key } );
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
		//[AutoValidateAntiforgeryToken]
		[ProducesResponseType( typeof( GetDeviceResponseDto ), StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> GetDevice( [FromRoute] Guid id, CancellationToken cancellationToken )
		{
			var response = await DeviceRepository.ReadAsync( id, cancellationToken );

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
		//[AutoValidateAntiforgeryToken]
		[ProducesResponseType( typeof( IEnumerable<GetDeviceResponseDto> ), StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		public async Task<IActionResult> GetDevices( CancellationToken cancellationToken )
		{
			var response = await DeviceRepository.ReadAllAsync( cancellationToken );

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
		//[AutoValidateAntiforgeryToken]
		[ProducesResponseType( StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status400BadRequest )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> UpdateDevice( [FromRoute] Guid id, [FromBody] CreateDeviceRequestDto dto, CancellationToken cancellationToken )
		{
			var device = Mapper.Map<Device>( dto );
			device.Key = id.ToString();

			var response = await DeviceRepository.UpdateAsync( device, cancellationToken );

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
		//[AutoValidateAntiforgeryToken]
		[ProducesResponseType( StatusCodes.Status200OK )]
		[ProducesResponseType( typeof( ErrorResponseDto ), StatusCodes.Status404NotFound )]
		public async Task<IActionResult> DeleteDevice( [FromRoute] Guid id, CancellationToken cancellationToken )
		{
			var response = await DeviceRepository.DeleteAsync( id, cancellationToken );

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
