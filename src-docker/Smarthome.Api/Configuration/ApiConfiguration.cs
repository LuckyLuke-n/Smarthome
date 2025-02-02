namespace Smarthome.Api.Configuration
{
	public class HealthEndpoint
	{
		public ServiceType Key { get; set; }
		public string Value { get; set; } = string.Empty;
	}

	public enum ServiceType
	{
		Influx,
		Grafana,
		RabbitMq,
		Emqx
	}

	public class ApiConfiguration
	{
		public static string Section => "Api";
		public List<HealthEndpoint> HealthEndpoints { get; set; } = [];
	}
}
