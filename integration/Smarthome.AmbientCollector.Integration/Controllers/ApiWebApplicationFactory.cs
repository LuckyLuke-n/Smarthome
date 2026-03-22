using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Smarthome.AmbientCollector.Api.Monitoring.MessageBus;
using Smarthome.AmbientCollector.Api.Monitoring.WeatherData;
using Smarthome.AmbientCollector.Api.Repositories.Locations;

namespace Smarthome.AmbientCollector.Integration.Controllers;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<ILocationRepository> LocationRepositoryMock { get; } = new();

    protected override void ConfigureWebHost( IWebHostBuilder builder )
    {
        builder.ConfigureServices( services =>
        {
            services.RemoveAll<ILocationRepository>();

            RemoveHostedService<DeviceMonitor>( services );
            RemoveHostedService<WeatherMonitor>( services );

            services.AddSingleton( LocationRepositoryMock.Object );

            services.AddLogging();
        } );
    }

    private static void RemoveHostedService<TService>( IServiceCollection services ) where TService : class, IHostedService
    {
        for ( var i = services.Count - 1; i >= 0; i-- )
        {
            var descriptor = services[i];
            if ( descriptor.ServiceType == typeof( IHostedService )
                 && descriptor.ImplementationType == typeof( TService ) )
            {
                services.RemoveAt( i );
            }
        }
    }
}
