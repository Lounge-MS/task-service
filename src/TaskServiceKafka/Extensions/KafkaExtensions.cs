using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskServiceDomain.Interfaces;
using TaskServiceKafka.Kafka.Consumers;

namespace TaskServiceKafka.Extensions;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaConsumer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string kafkaBootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        string kafkaTopic = configuration["Kafka:Topic"] ?? "TaskCreateTopic";

        services.AddSingleton<IConsumer<Ignore, string>>(sp =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaBootstrapServers,
                GroupId = "task-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
            };
            return new ConsumerBuilder<Ignore, string>(config).Build();
        });

        services.AddHostedService<EventConsumer>(sp =>
        {
            IConsumer<Ignore, string> consumer = sp.GetRequiredService<IConsumer<Ignore, string>>();
            ITaskService taskService = sp.GetRequiredService<ITaskService>();
            ILogger<EventConsumer> logger = sp.GetRequiredService<ILogger<EventConsumer>>();
            return new EventConsumer(consumer, taskService, logger, kafkaTopic);
        });

        return services;
    }
}