using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Smarthome.AmbientCollector.Integration.Repository;

public sealed class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;
    public string ConnectionString => _mongoContainer.GetConnectionString();

    public MongoDbFixture()
    {
        _mongoContainer = new MongoDbBuilder("mongo:7.0")
            .WithCleanUp(true)
            .Build();
    }


    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }

    public IMongoClient CreateClient()
    {
        return new MongoClient(ConnectionString);
    }
}