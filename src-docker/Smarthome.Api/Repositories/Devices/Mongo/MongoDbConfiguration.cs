namespace Smarthome.Api.Repositories.Devices.Mongo
{
	public class MongoDbConfiguration
	{
		public static string Section => "MongoDb";
		public string ConnectionString { get; set; } = string.Empty;
		public static string ConnectionStringEnvVar => $"SMARTHOME_{Section}__{nameof( ConnectionString )}";
	}
}
