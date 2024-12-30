namespace Smarthome.Api.Repositories.Devices.Mongo
{
	public class MongoDbConfiguration
	{
		public static string Prefix => "MongoDb";
		public string ConnectionString { get; set; } = string.Empty;
	}
}
