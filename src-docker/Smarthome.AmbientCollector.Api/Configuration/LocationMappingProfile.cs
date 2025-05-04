using AutoMapper;
using Smarthome.Core.DomainObjects;
using Smarthome.Core.Models;

namespace Smarthome.AmbientCollector.Api.Configuration
{
	public class LocationMappingProfile : Profile
	{
		public LocationMappingProfile()
		{
			CreateMap<GetLocationResponseDto, Location>().ReverseMap();

			CreateMap<CreateLocationRequestDto, Location>()
				.ForMember( m => m.Id, o => o.Ignore() );

			CreateMap<UpdateLocationRequestDto, Location>()
				.ForMember( m => m.Id, o => o.Ignore() );
		}
	}
}
