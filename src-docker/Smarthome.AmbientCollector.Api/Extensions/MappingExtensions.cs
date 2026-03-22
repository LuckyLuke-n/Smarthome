using Smarthome.Core.DomainObjects;
using Smarthome.Core.Models;

namespace Smarthome.AmbientCollector.Api.Extensions;

public static class MappingExtensions
{
    public static Location ToEntity(this CreateLocationRequestDto dto)
    {
        return new Location
        {
            City = dto.City,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
    }

    public static Location ToEntity(this UpdateLocationRequestDto dto)
    {
        return new Location
        {
            City = dto.City,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
    }

    public static GetLocationResponseDto ToDto(this Location location)
    {
        return new GetLocationResponseDto
        {
            Id = location.Id,
            City = location.City,
            Latitude = location.Latitude,
            Longitude = location.Longitude
        };
    }
}