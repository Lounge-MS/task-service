using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskServiceInfrastructure.Infrastructure.Data;

namespace TaskServiceKafka.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddTaskServiceDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=TaskServiceDb;Username=postgres;Password=postgres";

        services.AddDbContext<TaskDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        TaskDbContext dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            Console.WriteLine("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
            throw;
        }
    }
}
