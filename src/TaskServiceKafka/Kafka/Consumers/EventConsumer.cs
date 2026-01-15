using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskServiceDomain.Interfaces;
using TaskServiceDomain.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskServiceKafka.Kafka.Consumers;

public class EventConsumer : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ITaskService _taskService;
    private readonly ILogger<EventConsumer> _logger;
    private readonly string _topic;
    private readonly JsonSerializerOptions _options;

    public EventConsumer(
        IConsumer<Ignore, string> consumer,
        ITaskService taskService,
        ILogger<EventConsumer> logger,
        string topic = "TaskCreateTopic")
    {
        _consumer = consumer;
        _taskService = taskService;
        _logger = logger;
        _topic = topic;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Subscribed to topic: {Topic}", _topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<Ignore, string>? result = _consumer.Consume(stoppingToken);
                    if (result?.Message?.Value == null)
                        continue;

                    _logger.LogInformation(
                        "Received message from topic {Topic}: {Message}",
                        _topic,
                        result.Message.Value);

                    await ProcessMessageAsync(result.Message.Value, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while consuming message");
                }
            }
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Consumer closed");
        }
    }

    private async Task ProcessMessageAsync(string messageJson, CancellationToken cancellationToken)
    {
        try
        {
            TaskCreateEvent? taskEvent = JsonSerializer.Deserialize<TaskCreateEvent>(messageJson, _options);
            if (taskEvent == null)
            {
                _logger.LogWarning("Failed to deserialize message: {Message}", messageJson);
                return;
            }

            await _taskService.CreateTaskAsync(
                taskEvent.Title,
                taskEvent.Description,
                taskEvent.Deadline,
                taskEvent.CreatedByUserId,
                taskEvent.Metadata,
                cancellationToken);

            _logger.LogInformation("Task created successfully from event: {Title}", taskEvent.Title);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message: {Message}", messageJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", messageJson);
        }
    }
}