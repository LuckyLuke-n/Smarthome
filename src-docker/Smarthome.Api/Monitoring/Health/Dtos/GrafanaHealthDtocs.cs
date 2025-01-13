namespace Smarthome.Api.Monitoring.Health.Dtos
{
	public class GrafanaHealthDtocs
	{
		public string Database { get; set; } = string.Empty;
		public string Version { get; set; } = string.Empty;
		public string Commit { get; set; } = string.Empty;

	}
}
