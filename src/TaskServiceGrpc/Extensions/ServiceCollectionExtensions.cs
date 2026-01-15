using TaskServiceDomain.Interfaces;
using TaskServiceDomain.Services;
using TaskServiceInfrastructure.Infrastructure.Repositories;

namespace TaskServiceGrpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskServiceRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }

    public static IServiceCollection AddTaskServices(this IServiceCollection services)
    {
        services.AddScoped<TaskFilterService>();
        services.AddScoped<ITaskService, TaskServiceDomain.Services.TaskService>();
        return services;
    }
}
