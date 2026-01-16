using TaskServiceGrpc.Controllers;
using TaskServiceGrpc.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddTaskServiceDatabase(builder.Configuration);

builder.Services.AddTaskServiceRepositories();

builder.Services.AddTaskServices();

builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.SetMinimumLevel(LogLevel.Information);
});

WebApplication app = builder.Build();

await app.Services.EnsureDatabaseCreatedAsync();

app.MapGrpcService<TaskServiceController>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
