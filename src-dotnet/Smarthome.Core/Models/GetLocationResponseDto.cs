namespace Smarthome.Core.Models
{
	public class GetLocationResponseDto
	{
		public string Id { get; set; } = string.Empty;

		public string City { get; set; } = string.Empty;
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}
