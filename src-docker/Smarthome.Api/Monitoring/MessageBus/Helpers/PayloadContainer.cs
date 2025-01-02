namespace Smarthome.Api.Monitoring.MessageBus.Helpers
{
	internal class PayloadContainer
	{
		public Payload Payload { get; set; } = new();
		public string Topic { get; set; } = string.Empty;
	}
}