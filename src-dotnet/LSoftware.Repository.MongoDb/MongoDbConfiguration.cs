namespace LSoftware.Repository.MongoDb
{
	public class MongoDbConfiguration
	{
		public static string Section => "MongoDb";
		public string ConnectionString { get; set; } = string.Empty;
		public static string ConnectionStringEnvVar => $"SMARTHOME_{Section}__{nameof( ConnectionString )}";
	}
}
