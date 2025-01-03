using AutoMapper;
using Smarthome.Core.DomainObjects;
using Smarthome.Core.Models;

namespace Smarthome.Api.Configuration
{
	public class DeviceMappingProfile : Profile
	{
		public DeviceMappingProfile()
		{
			CreateMap<GetDeviceResponseDto, Device>().ReverseMap();

			CreateMap<CreateDeviceRequestDto, Device>()
				.ForMember( m => m.Id, o => o.Ignore() )
				.ForMember( m => m.DataSource, o => o.Ignore() )
				.ForMember( m => m.Hardware, o => o.MapFrom( src => new Hardware()
				{
					Model = src.Model,
					Type = src.Type,
				} ) );

			CreateMap<UpdateDeviceRequestDto, Device>()
				.ForMember( m => m.Id, o => o.Ignore() )
				.ForMember( m => m.DataSource, o => o.Ignore() )
				.ForMember( m => m.Hardware, o => o.MapFrom( src => new Hardware()
				{
					Model = src.Model,
					Type = src.Type,
				} ) );
		}
	}
}
