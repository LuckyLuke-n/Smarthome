using AutoMapper;
using Smarthome.AmbientCollector.Api.Monitoring.WeatherData;
using Smarthome.AmbientCollector.Api.Repositories.WeatherReport;

namespace Smarthome.AmbientCollector.Api.Configuration
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
	