using AutoMapper;
using Smarthome.Api.Monitoring.WeatherData;
using Smarthome.Api.Repositories.WeatherReport;

namespace Smarthome.Api.Configuration
{
	public class WeatherReportMappingProfile : Profile
	{
		public WeatherReportMappingProfile()
		{
			CreateMap<WeatherReport, WeatherReportData>()
				.ForMember( m => m.MeasurementName, o => o.Ignore() );
		}
	}
}
	