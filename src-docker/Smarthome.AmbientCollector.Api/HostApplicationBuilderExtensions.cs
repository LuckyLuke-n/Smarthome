using LSoftware.Repository.MongoDb;
using Smarthome.AmbientCollector.Api.Repositories.Locations;

namespace Smarthome.AmbientCollector.Api;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDbRepostiroy();
        builder.Services.AddSingleton<ILocationRepository, LocationMongoRepository>();
        
        return builder;
    }
}