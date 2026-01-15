namespace TaskServiceDomain.Models;

public class TaskCreateEvent
{
    public required string Title { get; set; }

    public required string Description { get; set; }

    public required DateTime Deadline { get; set; }

    public required long CreatedByUserId { get; set; }

    public TaskMetadata? Metadata { get; set; }
}