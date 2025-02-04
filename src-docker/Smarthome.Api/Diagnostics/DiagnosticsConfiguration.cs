namespace Smarthome.AmbientCollector.Api.Diagnostics
{
	public class DiagnosticsConfiguration
	{
		public static string Section => "Diagnostics";
		public bool TracingEnabled { get; set; }
		public string OtlpEndpoint { get; set; } = string.Empty;
		public static string TracingEnabledEnvVar => $"SMARTHOME_{Section}__{nameof( TracingEnabled )}";
		public static string OtlpEndpointEnvVar => $"SMARTHOME_{Section}__{nameof( OtlpEndpoint )}";
	}
}
