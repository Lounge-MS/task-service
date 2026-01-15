namespace TaskServiceDomain.Models;

public class TaskHistory(long userId, HistoryType historyType, DateTime timestamp)
{
    public long UserId { get; set; } = userId;

    public HistoryType HistoryType { get; set; } = historyType;

    public DateTime Timestamp { get; set; } = timestamp;
}