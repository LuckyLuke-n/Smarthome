namespace Smarthome.Api.Diagnostics
{
	public class DiagnosticsConfiguration
	{
		public static string Section => "Diagnostics";
		public bool IsEnabled { get; set; }
		public string ConnectionString { get; set; } = string.Empty;
		public static string IsEnabledEnvVar => $"SMARTHOME_{Section}__{nameof( IsEnabled )}";
	}
}
