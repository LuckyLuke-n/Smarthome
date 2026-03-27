using LSoftware.Repository.MongoDb;
using Microsoft.Extensions.Logging;
using Moq;
using Smarthome.AmbientCollector.Api.Repositories.Locations;
using Smarthome.Core.DomainObjects;

namespace Smarthome.AmbientCollector.Integration.Repository;

public class LocationRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _fixture;

    public LocationRepositoryTests( MongoDbFixture fixture )
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_WhenLocationIsValid_ShouldInsertAndReturnSuccess()
    {
        var repository = CreateRepository();
        await CleanupAsync();

        var location = new Location
        {
            City = "Berlin",
            Latitude = 52.52,
            Longitude = 13.405
        };

        var response = await repository.CreateAsync( location );

        Assert.True( response.IsSuccess );
        Assert.NotNull( response.ValueSuccess );
        Assert.NotEqual( string.Empty, response.ValueSuccess!.Id );
        Assert.Equal( "Berlin", response.ValueSuccess.City );
        Assert.Equal( 52.52, response.ValueSuccess.Latitude );
        Assert.Equal( 13.405, response.ValueSuccess.Longitude );

        var readBack = await repository.ReadAsync( Guid.Parse( response.ValueSuccess.Id ) );

        Assert.True( readBack.IsSuccess );
        Assert.NotNull( readBack.ValueSuccess );
        Assert.Equal( response.ValueSuccess.Id, readBack.ValueSuccess!.Id );
        Assert.Equal( "Berlin", readBack.ValueSuccess.City );
    }

    [Fact]
    public async Task ReadAllAsync_WhenCollectionHasMultipleDocuments_ShouldReturnAllDocuments()
    {
        var repository = CreateRepository();
        await CleanupAsync();

        await repository.CreateAsync( new Location { City = "Berlin", Latitude = 52.52, Longitude = 13.405 } );
        await repository.CreateAsync( new Location { City = "Munich", Latitude = 48.1351, Longitude = 11.5820 } );

        var response = await repository.ReadAllAsync();

        Assert.True( response.IsSuccess );
        Assert.NotNull( response.ValueSuccess );

        var locations = response.ValueSuccess!.ToList();
        Assert.Equal( 2, locations.Count );
        Assert.Contains( locations, x => x.City == "Berlin" );
        Assert.Contains( locations, x => x.City == "Munich" );
    }

    [Fact]
    public async Task ReadAsync_WhenDocumentDoesNotExist_ShouldReturnFail()
    {
        var repository = CreateRepository();
        await CleanupAsync();

        var response = await repository.ReadAsync( Guid.NewGuid() );

        Assert.False( response.IsSuccess );
        Assert.NotNull( response.ValueFail );
    }

    [Fact]
    public async Task UpdateAsync_WhenDocumentExists_ShouldPersistChanges()
    {
        var repository = CreateRepository();
        await CleanupAsync();

        var created = await repository.CreateAsync( new Location
        {
            City = "Hamburg",
            Latitude = 53.5511,
            Longitude = 9.9937
        } );

        Assert.True( created.IsSuccess );
        Assert.NotNull( created.ValueSuccess );

        var locationToUpdate = created.ValueSuccess!;
        locationToUpdate.City = "Stuttgart";
        locationToUpdate.Latitude = 48.7758;
        locationToUpdate.Longitude = 9.1829;

        var updateResponse = await repository.UpdateAsync( locationToUpdate );

        Assert.True( updateResponse.IsSuccess );
        Assert.NotNull( updateResponse.ValueSuccess );
        Assert.Equal( "Hamburg", updateResponse.ValueSuccess!.City );

        var readBack = await repository.ReadAsync( Guid.Parse( locationToUpdate.Id ) );

        Assert.True( readBack.IsSuccess );
        Assert.NotNull( readBack.ValueSuccess );
        Assert.Equal( "Stuttgart", readBack.ValueSuccess!.City );
        Assert.Equal( 48.7758, readBack.ValueSuccess.Latitude );
        Assert.Equal( 9.1829, readBack.ValueSuccess.Longitude );
    }

    [Fact]
    public async Task UpdateAsync_WhenDocumentDoesNotExist_ShouldReturnFail()
    {
        var repository = CreateRepository();
        await CleanupAsync();

        var response = await repository.UpdateAsync( new Location
        {
            Id = Guid.NewGuid().ToString(),
            City = "Nowhere",
            Latitude = 0,
            Longitude = 0
        } );

        Assert.False( response.IsSuccess );
        Assert.NotNull( response.ValueFail );
    }

    [Fact]
    public async Task DeleteAsync_WhenDocumentExists_ShouldRemoveDocument()
    {
        var repository = CreateRepository();
        await CleanupAsync();

        var created = await repository.CreateAsync( new Location
        {
            City = "Cologne",
            Latitude = 50.9375,
            Longitude = 6.9603
        } );

        Assert.True( created.IsSuccess );
        Assert.NotNull( created.ValueSuccess );

        var deleteResponse = await repository.DeleteAsync( Guid.Parse( created.ValueSuccess!.Id ) );

        Assert.True( deleteResponse.IsSuccess );

        var readBack = await repository.ReadAsync( Guid.Parse( created.ValueSuccess.Id ) );

        Assert.False( readBack.IsSuccess );
    }

    [Fact]
    public async Task DeleteAsync_WhenDocumentDoesNotExist_ShouldReturnFail()
    {
        var repository = CreateRepository();
        await CleanupAsync();

        var response = await repository.DeleteAsync( Guid.NewGuid() );

        Assert.False( response.IsSuccess );
        Assert.NotNull( response.ValueFail );
    }

    private LocationMongoRepository CreateRepository()
    {
        var mongoClient = _fixture.CreateClient();
        var loggerMock = new Mock<ILogger<MongoDbRepository<Location>>>();

        return new LocationMongoRepository( mongoClient, loggerMock.Object );
    }

    private async Task CleanupAsync()
    {
        var client = _fixture.CreateClient();
        var database = client.GetDatabase( "Smarthome" );

        try
        {
            await database.DropCollectionAsync( nameof( Location ) );
        }
        catch
        {
            // collection may not exist yet
        }
    }
}