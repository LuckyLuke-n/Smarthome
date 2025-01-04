using AutoMapper;
using Smarthome.Core.DomainObjects;
using Smarthome.Core.Models;

namespace Smarthome.Api.Configuration
{
	public class LocationeMappingProfile : Profile
	{
		public LocationeMappingProfile()
		{
			CreateMap<GetLocationResponseDto, Location>().ReverseMap();

			CreateMap<CreateLocationRequestDto, Location>()
				.ForMember( m => m.Id, o => o.Ignore() );

			CreateMap<UpdateLocationRequestDto, Location>()
				.ForMember( m => m.Id, o => o.Ignore() );
		}
	}
}
