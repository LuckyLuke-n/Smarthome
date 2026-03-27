using System.Net;
using System.Net.Http.Json;
using LSoftware.Repository.Abstractions;
using Moq;
using Smarthome.Core.DomainObjects;
using Smarthome.Core.Models;

namespace Smarthome.AmbientCollector.Integration.Controllers;

public class LocationsControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly double _tolerance = 0.0001;

    public LocationsControllerTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddLocation_WhenRepositoryReturnsSuccess_ShouldReturnCreated()
    {
        var locationId = Guid.NewGuid().ToString();
        var request = new CreateLocationRequestDto
        {
            City = "Berlin",
            Latitude = 52.52,
            Longitude = 13.405
        };

        var mappedLocation = new Location
        {
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var createdLocation = new Location
        {
            Id = locationId,
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _factory.LocationRepositoryMock
            .Setup(x => x.CreateAsync(It.Is<Location>(l =>
                l.City == "Berlin" &&
                Math.Abs(l.Latitude - 52.52) <= _tolerance &&
                Math.Abs(l.Longitude - 13.405) <= _tolerance), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<Location, RepositoryFailResponse>.CreateSuccess(createdLocation));

        var response = await _client.PostAsJsonAsync("/api/locations", request);
        var returned = await response.Content.ReadFromJsonAsync<Location>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(returned);
        Assert.Equal(request.City, returned!.City);
        Assert.Equal(request.Latitude, returned.Latitude);
        Assert.Equal(request.Longitude, returned.Longitude);
        Assert.False(string.IsNullOrWhiteSpace(returned.Id));
    }

    [Fact]
    public async Task AddLocation_WhenRepositoryReturnsConflict_ShouldReturnConflict()
    {
        var request = new CreateLocationRequestDto
        {
            City = "Berlin",
            Latitude = 52.52,
            Longitude = 13.405
        };

        _factory.LocationRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<Location, RepositoryFailResponse>.CreateFail(
                new RepositoryFailResponse(HttpStatusCode.Conflict, "Already exists")));

        var response = await _client.PostAsJsonAsync("/api/locations", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetLocation_WhenRepositoryReturnsSuccess_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var location = new Location
        {
            Id = id.ToString(),
            City = "Hamburg",
            Latitude = 53.5511,
            Longitude = 9.9937
        };

        _factory.LocationRepositoryMock
            .Setup(x => x.ReadAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<Location, RepositoryFailResponse>.CreateSuccess(location));

        var response = await _client.GetAsync($"/api/locations/{id}");
        var returned = await response.Content.ReadFromJsonAsync<Location>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(returned);
        Assert.Equal(location.City, returned!.City);
        Assert.Equal(location.Latitude, returned.Latitude);
        Assert.Equal(location.Longitude, returned.Longitude);
        Assert.False(string.IsNullOrWhiteSpace(returned.Id));
    }

    [Fact]
    public async Task GetLocation_WhenRepositoryReturnsNotFound_ShouldReturnNotFound()
    {
        var id = Guid.NewGuid();

        _factory.LocationRepositoryMock
            .Setup(x => x.ReadAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<Location, RepositoryFailResponse>.CreateFail(
                new RepositoryFailResponse(HttpStatusCode.NotFound, "Not found")));

        var response = await _client.GetAsync($"/api/locations/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetLocations_WhenRepositoryReturnsSuccess_ShouldReturnOk()
    {
        _factory.LocationRepositoryMock
            .Setup(x => x.ReadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<IEnumerable<Location>, RepositoryFailResponse>.CreateSuccess(
                new[]
                {
                    new Location
                        { Id = Guid.NewGuid().ToString(), City = "Berlin", Latitude = 52.52, Longitude = 13.405 },
                    new Location
                        { Id = Guid.NewGuid().ToString(), City = "Munich", Latitude = 48.1351, Longitude = 11.5820 }
                }));

        var response = await _client.GetAsync("/api/locations");
        var locations = await response.Content.ReadFromJsonAsync<List<Location>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(locations);
        Assert.Equal(2, locations!.Count);
    }

    [Fact]
    public async Task GetLocations_WhenRepositoryReturnsBadRequest_ShouldReturnBadRequest()
    {
        _factory.LocationRepositoryMock
            .Setup(x => x.ReadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<IEnumerable<Location>, RepositoryFailResponse>.CreateFail(
                new RepositoryFailResponse(HttpStatusCode.BadRequest, "Invalid request")));

        var response = await _client.GetAsync("/api/locations");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocation_WhenRepositoryReturnsSuccess_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var request = new UpdateLocationRequestDto
        {
            City = "Stuttgart",
            Latitude = 48.7758,
            Longitude = 9.1829
        };

        _factory.LocationRepositoryMock
            .Setup(x => x.UpdateAsync(It.Is<Location>(l =>
                l.Id == id.ToString() &&
                l.City == "Stuttgart" &&
                Math.Abs(l.Latitude - 48.7758) <= _tolerance &&
                Math.Abs(l.Longitude - 9.1829) <= _tolerance), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<Location, RepositoryFailResponse>.CreateSuccess(new Location
            {
                Id = id.ToString(),
                City = request.City,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            }));

        var response = await _client.PutAsJsonAsync($"/api/locations/{id}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocation_WhenRepositoryReturnsNotFound_ShouldReturnNotFound()
    {
        var id = Guid.NewGuid();
        var request = new UpdateLocationRequestDto
        {
            City = "Nowhere",
            Latitude = 0,
            Longitude = 0
        };

        _factory.LocationRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<Location, RepositoryFailResponse>.CreateFail(
                new RepositoryFailResponse(HttpStatusCode.NotFound, "Missing")));

        var response = await _client.PutAsJsonAsync($"/api/locations/{id}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLocation_WhenRepositoryReturnsSuccess_ShouldReturnOk()
    {
        var id = Guid.NewGuid();

        _factory.LocationRepositoryMock
            .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<RepositoryFailResponse>.CreateSuccess());

        var response = await _client.DeleteAsync($"/api/locations/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLocation_WhenRepositoryReturnsNotFound_ShouldReturnNotFound()
    {
        var id = Guid.NewGuid();

        _factory.LocationRepositoryMock
            .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RepositoryResponse<RepositoryFailResponse>.CreateFail(
                new RepositoryFailResponse(HttpStatusCode.NotFound, "Missing")));

        var response = await _client.DeleteAsync($"/api/locations/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}