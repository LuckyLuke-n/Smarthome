using System.ComponentModel.DataAnnotations;

namespace Smarthome.Core.Models
{
	public class CreateLocationRequestDto
	{
		[Required]
		public string City { get; set; } = string.Empty;
		[Required]
		public double Latitude { get; set; }
		[Required]
		public double Longitude { get; set; }
	}
}
