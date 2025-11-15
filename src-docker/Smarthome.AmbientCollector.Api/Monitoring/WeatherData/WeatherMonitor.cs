using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Smarthome.AmbientCollector.Api.Diagnostics.Meters;
using Smarthome.AmbientCollector.Api.Repositories.Locations;
using Smarthome.AmbientCollector.Api.Repositories.WeatherReport;
using Smarthome.AmbientCollector.Api.Repositories.WeatherReport.Api;

namespace Smarthome.AmbientCollector.Api.Monitoring.WeatherData
{
	public class WeatherMonitor : IHostedService
	{
		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private Timer WeatherApiTimer { get; }
		private IWeatherRepository WeatherRepository { get; }
		private ILocationRepository LocationRepository { get; }
		private ILogger<WeatherMonitor> Logger { get; }
		private int ApiRefreshRateInMinutes { get; }

		public WeatherMonitor(IWeatherRepository weatherRepository,
			ILocationRepository locationRepository,
			IOptions<WeatherApiConfiguration> weatherApiOptions,
			ILogger<WeatherMonitor> logger )
		{
			WeatherApiTimer = new( TriggerWeatherTimerActionsAsync, null, int.MaxValue, int.MaxValue );
			WeatherRepository = weatherRepository;
			LocationRepository = locationRepository;
			Logger = logger;
			ApiRefreshRateInMinutes = weatherApiOptions.Value.RefreshIntervalInMinutes;
		}

		private async void TriggerWeatherTimerActionsAsync( object? state )
		{
			if ( CancellationTokenSource.IsCancellationRequested )
				return;

			await GetWeatherDataAsync().ConfigureAwait( false );
		}

		private async Task GetWeatherDataAsync()
		{
			var repositoryResponse = await LocationRepository.ReadAllAsync().ConfigureAwait( false );

			if ( !repositoryResponse.IsSuccess )
			{
				Logger.LogWarning( "Could not retrieve locations from repository." );
				return;
			}

			var locations = repositoryResponse.ValueSuccess!;

			foreach ( var location in locations )
			{
				var response = await WeatherRepository.GetWeatherDataAsync( location, CancellationTokenSource.Token ).ConfigureAwait( false );

				if ( !response.IsSuccess )
				{
					Logger.LogWarning( "Could not retrieve weather data from WeatherRepository" );
					return;
				}

				if ( CancellationTokenSource.IsCancellationRequested )
					return;

				var weatherReport = response.ValueSuccess!;
				WeatherMeter.Update( weatherReport, weatherReport.Location );
			}
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
