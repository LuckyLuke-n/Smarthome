using Smarthome.Core.DomainObjects;
using System.ComponentModel.DataAnnotations;

namespace Smarthome.Core.Models
{
	public class UpdateDeviceRequestDto
	{
		[Required]
		public string Hostname { get; set; } = string.Empty;

		[Required]
		public string Model { get; set; } = string.Empty;

		[Required]
		[EnumDataType( typeof( HardwareType ) )]
		public HardwareType Type { get; set; }

		[Required]
		[EnumDataType( typeof( State ) )]
		public State State { get; set; }

		[Required]
		[StringLength( maximumLength: 100, MinimumLength = 3, ErrorMessage = "Location name must have between 3 and 100 characters." )]
		public string Location { get; set; } = string.Empty;
	}
}
