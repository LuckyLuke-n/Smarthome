namespace Smarthome.Api.Repositories.Devices.Mongo
{
	public class MongoDbConfiguration
	{
		public static string Prefix => "MongoDb";

		public string Host {  get; set; } = string.Empty;
		public int Port {  get; set; }
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;

		public string ConnectionString => $"mongodb://{Username}:{Password}@{Host}:{Port}/";
	}
}
