using System.Text.Json;

namespace TaskServiceDomain.Models;

public class Task
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public Task(
        long id,
        string title,
        string description,
        DateTime deadline,
        TaskStatus status,
        DateTime createdAt,
        DateTime updatedAt,
        string? metadataJson = null)
    {
        Id = id;
        Title = title;
        Description = description;
        Deadline = deadline;
        Status = status;
        History = new List<TaskHistory>();
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        MetadataJson = metadataJson;
    }

    public long Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime Deadline { get; set; }

    public TaskStatus Status { get; set; }

    public IList<TaskHistory> History { get; } = new List<TaskHistory>();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? MetadataJson { get; set; }

    public TaskMetadata? GetMetadata()
    {
        if (string.IsNullOrEmpty(MetadataJson))
            return null;

        return JsonSerializer.Deserialize<TaskMetadata>(MetadataJson, _jsonSerializerOptions);
    }

    public void SetMetadata(TaskMetadata metadata)
    {
        MetadataJson = JsonSerializer.Serialize(metadata, _jsonSerializerOptions);
    }
}