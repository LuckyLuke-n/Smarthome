using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var mongo = builder.AddMongoDB("smarthome-mongo")
    .WithDataBindMount("data/mongo")
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitUser = builder.AddParameter("rabbit-user", value: "guest");
var rabbitPassword = builder.AddParameter("rabbit-password", value: "ZdMVWXxa15bVpxMpNesbyt", secret: true);
var rabbitmq = builder.AddRabbitMQ("smarthome-mqtt", rabbitUser, rabbitPassword)
    .WithDataBindMount("data/rabbitmq")
    .WithBindMount("devops/rabbitmq/enabled_plugins", "/etc/rabbitmq/enabled_plugins")
    .WithManagementPlugin()
    .WithEndpoint(port: 1883, targetPort: 1883, scheme:"mqtt")  // MQTT standard port
    .WithLifetime(ContainerLifetime.Persistent);

var ambientcollector = builder.AddProject<Smarthome_AmbientCollector_Api>("ambientcollector")
    .WaitFor(mongo)
    .WaitFor(rabbitmq)
    .WithReference(mongo)
    .WithEnvironment("ConnectionStrings__smarthome-mqtt", "mqtt://guest:ZdMVWXxa15bVpxMpNesbyt@localhost:1883")
    .WithEnvironment("WeatherApi__Endpoint", "https://api.tomorrow.io/v4/weather/realtime")
    .WithEnvironment("WeatherApi__ApiKey", "key")
    .WithEnvironment("WeatherApi__RefreshIntervalInMinutes", "5");

builder.Build().Run();