namespace Smarthome.Api.Monitoring.Health.Dtos
{
	public class InfluxDbHealthDto
	{
		public string Name { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string Version { get; set; } = string.Empty;
		public string Commit { get; set; } = string.Empty;
	}
}
