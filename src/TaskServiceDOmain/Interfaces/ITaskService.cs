using TaskServiceDomain.Models;
using Task = TaskServiceDomain.Models.Task;
using TaskStatus = TaskServiceDomain.Models.TaskStatus;

namespace TaskServiceDomain.Interfaces;

public interface ITaskService
{
    Task<Task> CreateTaskAsync(
        string title,
        string description,
        DateTime deadline,
        long createdByUserId,
        TaskMetadata? metadata,
        CancellationToken cancellationToken);

    Task<Task?> CancelTaskAsync(
        long taskId,
        long canceledByUserId,
        CancellationToken cancellationToken);

    Task<Task?> CompleteTaskAsync(
        long taskId,
        long completedByUserId,
        CancellationToken cancellationToken);

    Task<Task?> UpdateTaskStatusAsync(
        long taskId,
        TaskStatus newStatus,
        long updatedByUserId,
        CancellationToken cancellationToken);

    Task<IEnumerable<Task>> GetTasksWithFiltersAsync(
        TaskStatus? status,
        CancellationToken cancellationToken);

    Task<Task?> GetTaskByIdAsync(long id, CancellationToken cancellationToken);

    Task<IEnumerable<TaskHistory>> GetTaskHistoryAsync(long taskId, CancellationToken cancellationToken);
}