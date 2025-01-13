using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Smarthome.Api;
using Smarthome.Api.Configuration;
using Smarthome.Api.Diagnostics;
using Smarthome.Api.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder( args );

// Environment varaibles into configuration provider
builder.Configuration.AddEnvironmentVariables( prefix: "SMARTHOME_" );

#region Logging
LogLevel logLevel = LogLevel.Warning;
if ( !string.IsNullOrEmpty( Environment.GetEnvironmentVariable( "SMARTHOME_Api__LogLevel" ) ) )
	logLevel = ( LogLevel )int.Parse( Environment.GetEnvironmentVariable( "SMARTHOME_Api__LogLevel" )! );

builder.Services.AddLogging( loggingBuilder =>
	{
		loggingBuilder.AddConsole();
		loggingBuilder.SetMinimumLevel( logLevel );
	} );
builder.Logging.AddFilter( "System.Net.Http.HttpClient", logLevel );
#endregion

builder.Services.AddAutoMapper( typeof( DeviceMappingProfile ), typeof( WeatherReportMappingProfile ) )
	.AddMyServices( builder.Configuration );

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
	options.SwaggerDoc( "v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Smarthome API",
		Description = "An ASP.NET Core Web API for managing the smarthome.",
		License = new OpenApiLicense
		{
			Name = "BSD-3-Clause",
#pragma warning disable S1075 // URIs should not be hardcoded
			Url = new Uri( "https://opensource.org/licenses/BSD-3-Clause" )
#pragma warning restore S1075 // URIs should not be hardcoded
		}
	} );
} );

builder.AddOpenTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ApiKeyMiddleware>();

if ( app.Environment.IsDevelopment() )
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait( false );
