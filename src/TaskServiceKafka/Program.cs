using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskService.Extensions;
using TaskServiceKafka.Extensions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddTaskServiceDatabase(configuration);

builder.Services.AddTaskServiceRepositories();

builder.Services.AddTaskServices();

builder.Services.AddKafkaConsumer(configuration);

builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.SetMinimumLevel(LogLevel.Information);
});

IHost host = builder.Build();

await host.Services.EnsureDatabaseCreatedAsync();

string kafkaBootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
string kafkaTopic = configuration["Kafka:Topic"] ?? "TaskCreateTopic";
string connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=TaskServiceDb;Username=postgres;Password=postgres";

Console.WriteLine("Task Service is starting...");
Console.WriteLine($"Kafka Bootstrap Servers: {kafkaBootstrapServers}");
Console.WriteLine($"Kafka Topic: {kafkaTopic}");
Console.WriteLine($"Database Connection: {connectionString}");

await host.RunAsync();