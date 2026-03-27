using Smarthome.AmbientCollector.Api.Extensions;
using Smarthome.Core.DomainObjects;
using Smarthome.Core.Models;

namespace Smarthome.AmbientCollector.Test;

public class MappingExtensionsTests
{
    [Fact]
    public void ToEntity_FromCreateLocationRequestDto_ShouldMapAllFields()
    {
        var dto = new CreateLocationRequestDto
        {
            City = "Berlin",
            Latitude = 52.52,
            Longitude = 13.405
        };

        var entity = dto.ToEntity();

        Assert.NotNull(entity);
        Assert.Equal(dto.City, entity.City);
        Assert.Equal(dto.Latitude, entity.Latitude);
        Assert.Equal(dto.Longitude, entity.Longitude);
        Assert.True(string.IsNullOrEmpty(entity.Id));
    }

    [Fact]
    public void ToEntity_FromUpdateLocationRequestDto_ShouldMapAllFields()
    {
        var dto = new UpdateLocationRequestDto
        {
            City = "Munich",
            Latitude = 48.1351,
            Longitude = 11.582
        };

        var entity = dto.ToEntity();

        Assert.NotNull(entity);
        Assert.Equal(dto.City, entity.City);
        Assert.Equal(dto.Latitude, entity.Latitude);
        Assert.Equal(dto.Longitude, entity.Longitude);
        Assert.True(string.IsNullOrEmpty(entity.Id));
    }

    [Fact]
    public void ToDto_FromLocation_ShouldMapAllFields()
    {
        var location = new Location
        {
            Id = Guid.NewGuid().ToString(),
            City = "Hamburg",
            Latitude = 53.5511,
            Longitude = 9.9937
        };

        var dto = location.ToDto();

        Assert.NotNull(dto);
        Assert.Equal(location.Id, dto.Id);
        Assert.Equal(location.City, dto.City);
        Assert.Equal(location.Latitude, dto.Latitude);
        Assert.Equal(location.Longitude, dto.Longitude);
    }
}