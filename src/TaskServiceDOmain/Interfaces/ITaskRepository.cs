using Task = TaskServiceDomain.Models.Task;
using TaskStatus = TaskServiceDomain.Models.TaskStatus;

namespace TaskServiceDomain.Interfaces;

public interface ITaskRepository
{
    Task<Task?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Task> CreateAsync(Task task, CancellationToken cancellationToken);

    Task<Task> UpdateAsync(Task task, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);

    Task<IEnumerable<Task>> GetAllAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Task>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken);
}