namespace Smarthome.Core.DomainObjects
{
	[Serializable]
	public class Device
	{
		public Guid Key { get; set; }
		public string Hostname { get; set; } = string.Empty;
		public Hardware Hardware { get; set; } = new();
		public string Location { get; set; } = string.Empty;
		public string DataSource { get; set; } = string.Empty;
	}
}
