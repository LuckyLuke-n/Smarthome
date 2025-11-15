using LSoftware.Repository.MongoDb;
using Microsoft.OpenApi.Models;
using Smarthome.AmbientCollector.Api;
using Smarthome.AmbientCollector.Api.Configuration;
using Smarthome.AmbientCollector.Api.Diagnostics;

var builder = WebApplication.CreateBuilder( args );

builder.AddServiceDefaults();

// Environment variables into configuration provider
builder.Configuration.AddEnvironmentVariables( prefix: "SMARTHOME_" );
builder.Services.Configure<MongoDbConfiguration>( builder.Configuration.GetSection( DiagnosticsConfiguration.Section ) );

builder.Services.AddLogging();
builder.Logging.ClearProviders();

builder.Services.AddAmbientCollectorOpenTelemetry();

builder.Services.AddAutoMapper(typeof(LocationMappingProfile));
builder.Services.AddMyServices( builder.Configuration );

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


var app = builder.Build();

app.MapDefaultEndpoints();

if ( app.Environment.IsDevelopment() )
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
// app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait( false );
