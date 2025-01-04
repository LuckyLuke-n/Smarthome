
using AutoMapper;
using LSoftware.Metrics.Abstractions;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using Smarthome.Api.Repositories.WeatherReport;
using Smarthome.Api.Repositories.WeatherReport.Api;

namespace Smarthome.Api.Monitoring.WeatherData
{
	public class WeatherMonitor : IHostedService
	{
		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private Timer WeatherApiTimer { get; }
		private IMetricsLogger<WeatherReportData> WeatherLogger { get; }
		private IWeatherRepository WeatherRepository { get; }
		private IMapper Mapper { get; }
		private ILogger<WeatherMonitor> Logger { get; }
		private int ApiRefreshRateInMinutes;

		public WeatherMonitor( IMetricsLogger<WeatherReportData> weatherLogger,
			IWeatherRepository weatherRepository,
			IMapper mapper,
			IOptions<WeatherApiConfiguration> weatherApiOptions,
			ILogger<WeatherMonitor> logger )
		{
			WeatherApiTimer = new( TriggerWeatherTimerActionsAsync, null, int.MaxValue, int.MaxValue );
			WeatherLogger = weatherLogger;
			WeatherRepository = weatherRepository;
			Mapper = mapper;
			Logger = logger;
			ApiRefreshRateInMinutes = weatherApiOptions.Value.IntervalInMinutes;
		}

		private async void TriggerWeatherTimerActionsAsync( object? state )
		{
			if ( CancellationTokenSource.IsCancellationRequested )
				return;

			await GetWeatherDataAsync().ConfigureAwait( false );
		}

		private async Task GetWeatherDataAsync()
		{
			var response = await WeatherRepository.GetWeatherDataAsync( CancellationTokenSource.Token ).ConfigureAwait( false );

			if ( !response.IsSuccess )
			{
				Logger.LogWarning( "Could not retrieve weather data from WeatherRepository" );
				return;
			}

			if ( CancellationTokenSource.IsCancellationRequested )
				return;

			var weatherReportData = Mapper.Map<WeatherReportData>( response.ValueSuccess! );
			WeatherLogger.SendInstant( weatherReportData );
		}

		public async Task StartAsync( CancellationToken cancellationToken )
		{
			await GetWeatherDataAsync().ConfigureAwait( false );
			WeatherApiTimer.Change( TimeSpan.FromSeconds( 1 ), TimeSpan.FromMinutes( ApiRefreshRateInMinutes ) );
			await Task.CompletedTask.ConfigureAwait( false );
		}

		public async Task StopAsync( CancellationToken cancellationToken )
		{
			await CancellationTokenSource.CancelAsync().ConfigureAwait( false );
			await WeatherApiTimer.DisposeAsync().ConfigureAwait( false );
		}
	}
}
