using Smarthome.Api;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddMyServices();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
////if ( app.Environment.IsDevelopment() )
////{
////	app.MapOpenApi();
////}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait( false );
