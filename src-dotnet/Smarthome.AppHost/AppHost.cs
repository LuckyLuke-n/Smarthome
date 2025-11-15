using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var mongo = builder.AddMongoDB("smarthome-mongo")
    .WithDataBindMount("data/mongo")
    .WithLifetime(ContainerLifetime.Persistent);

var emqx = builder.AddContainer("smarthome-emqx", "emqx/emqx")
    .WithBindMount("data/emqx", "/opt/emqx/data")
    .WithEndpoint(port: 1883, targetPort: 1883, scheme:"tcp")  // MQTT standard port
    .WithEndpoint(port: 18083, targetPort: 18083, scheme:"http")  // EMQX dashboard port
    .WithLifetime(ContainerLifetime.Persistent);

var ambientcollector = builder.AddProject<Smarthome_AmbientCollector_Api>("ambientcollector")
    .WaitFor(mongo)
    .WaitFor(emqx)
    .WithReference(mongo)
    .WithEnvironment("ConnectionStrings__smarthome-mqtt", "mqtt://admin:public@localhost:1883")
    .WithEnvironment("WeatherApi__Endpoint", "https://api.tomorrow.io/v4/weather/realtime")
    .WithEnvironment("WeatherApi__ApiKey", "key")
    .WithEnvironment("WeatherApi__RefreshIntervalInMinutes", "5");

builder.Build().Run();