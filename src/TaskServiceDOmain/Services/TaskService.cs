using TaskServiceDomain.Interfaces;
using TaskServiceDomain.Models;
using Task = TaskServiceDomain.Models.Task;
using TaskStatus = TaskServiceDomain.Models.TaskStatus;

namespace TaskServiceDomain.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly TaskFilterService _taskFilterService;

    public TaskService(ITaskRepository taskRepository, TaskFilterService taskFilterService)
    {
        _taskRepository = taskRepository;
        _taskFilterService = taskFilterService;
    }

    public async Task<Task> CreateTaskAsync(
        string title,
        string description,
        DateTime deadline,
        long createdByUserId,
        TaskMetadata? metadata = null,
        CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        var task = new Task(
            id: 0,
            title: title,
            description: description,
            deadline: deadline,
            status: TaskStatus.ACTUAL,
            history: new List<TaskHistory>
            {
                new TaskHistory(createdByUserId, HistoryType.CREATED, now),
            },
            createdAt: now,
            updatedAt: now);

        if (metadata != null)
        {
            task.SetMetadata(metadata);
        }

        return await _taskRepository.CreateAsync(task, cancellationToken);
    }

    public async Task<Task?> CancelTaskAsync(
        long taskId,
        long canceledByUserId,
        CancellationToken cancellationToken = default)
    {
        return await UpdateTaskStatusAsync(taskId, TaskStatus.CANCELED, canceledByUserId, cancellationToken);
    }

    public async Task<Task?> CompleteTaskAsync(
        long taskId,
        long completedByUserId,
        CancellationToken cancellationToken = default)
    {
        return await UpdateTaskStatusAsync(taskId, TaskStatus.COMPLETED, completedByUserId, cancellationToken);
    }

    Task<IEnumerable<Task>> ITaskService.GetTasksWithFiltersAsync(TaskStatus? status, CancellationToken cancellationToken)
    {
        return GetTasksWithFiltersAsync(status, cancellationToken);
    }

    public async Task<Task?> UpdateTaskStatusAsync(
        long taskId,
        TaskStatus newStatus,
        long updatedByUserId,
        CancellationToken cancellationToken)
    {
        Task? task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            return null;

        if (task.Status == newStatus)
            return task;

        TaskStatus oldStatus = task.Status;
        task.Status = newStatus;

        HistoryType historyType = newStatus switch
        {
            TaskStatus.COMPLETED => HistoryType.COMPLETED,
            TaskStatus.CANCELED => HistoryType.CANCELED,
            TaskStatus.ACTUAL => throw new NotImplementedException(),
            _ => throw new ArgumentException($"Unsupported status transition from {oldStatus} to {newStatus}"),
        };

        task.History.Add(new TaskHistory(updatedByUserId, historyType, DateTime.UtcNow));
        task.UpdatedAt = DateTime.UtcNow;

        return await _taskRepository.UpdateAsync(task, cancellationToken);
    }

    public async Task<IEnumerable<Task>> GetTasksWithFiltersAsync(
        TaskStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Task> tasks = await _taskRepository.GetAllAsync(cancellationToken);
        return _taskFilterService.FilterTasks(tasks, status);
    }

    public async Task<Task?> GetTaskByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TaskHistory>> GetTaskHistoryAsync(long taskId, CancellationToken cancellationToken = default)
    {
        Task? task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            return Enumerable.Empty<TaskHistory>();

        return task.History.OrderBy(h => h.Timestamp);
    }
}
