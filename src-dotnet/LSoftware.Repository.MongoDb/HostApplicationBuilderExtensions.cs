using Microsoft.Extensions.Hosting;

namespace LSoftware.Repository.MongoDb;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddMongoDbRepostiroy(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient(connectionName: "smarthome-mongo");
        
        return builder;
    }
}