using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var mongo = builder.AddMongoDB("smarthome-mongo")
    .WithDataBindMount("data/mongo")
    .WithLifetime(ContainerLifetime.Persistent);

var emqx = builder.AddContainer("ambientcollector-emqx", "emqx/emqx")
    .WithBindMount("data/emqx", "/opt/emqx/data")
    .WithEndpoint(port: 1883, targetPort: 1883, scheme:"mqtt")  // MQTT standard port
    .WithEndpoint(port: 18083, targetPort: 18083, scheme:"http")  // EMQX dashboard port
    .WithLifetime(ContainerLifetime.Persistent);

var ambientcollector = builder.AddProject<Smarthome_AmbientCollector_Api>("ambientcollector")
    .WaitFor(mongo)
    .WaitFor(emqx)
    .WithReference(mongo)
    .WithEnvironment("SMARTHOME_Api__LogLevel", "3")
    .WithEnvironment("SMARTHOME_Api__Key", "secret")
    .WithEnvironment("SMARTHOME_MongoDb__ConnectionString", "mongodb://localhost:27017")
    .WithEnvironment("SMARTHOME_MongoDb__Database", "smarthome")
    .WithEnvironment("SMARTHOME_Mqtt__Host", "localhost")
    .WithEnvironment("SMARTHOME_Mqtt__Port", "1883")
    .WithEnvironment("SMARTHOME_Mqtt__Username", "admin")
    .WithEnvironment("SMARTHOME_Mqtt__Password", "smarthome")
    .WithEnvironment("SMARTHOME_Mqtt__UseTls", "false")
    .WithEnvironment("SMARTHOME_WeatherApi__Endpoint", "https://api.tomorrow.io/v4/weather/realtime")
    .WithEnvironment("SMARTHOME_WeatherApi__ApiKey", "key")
    .WithEnvironment("SMARTHOME_WeatherApi__RefreshIntervalInMinutes", "5")
    .WithEnvironment("SMARTHOME_Diagnostics__TracingEnabled", "true")
    .WithEnvironment("SMARTHOME_Diagnostics__OtlpEndpoint", "http://localhost:4317");

var prometheus = builder.AddContainer("ambientcollector-prometheus", "prom/prometheus")
    .WithBindMount(source: "data/prometheus", target: "/prometheus")
    .WithBindMount(source: "devops/prometheus/prometheus.yml", target: "/etc/prometheus/prometheus.yml")
    .WithArgs("--config.file=/etc/prometheus/prometheus.yml")
    .WaitFor(ambientcollector);

builder.Build().Run();