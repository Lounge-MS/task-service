using TaskServiceDomain.Models;
using Task = TaskServiceDomain.Models.Task;
using TaskStatus = TaskServiceDomain.Models.TaskStatus;

namespace TaskServiceDomain.Services;

public class TaskFilterService
{
    public IEnumerable<Task> FilterTasks(
        IEnumerable<Task> tasks,
        TaskStatus? status)
    {
        IEnumerable<Task> filteredTasks = tasks.AsEnumerable();

        if (status.HasValue)
        {
            filteredTasks = filteredTasks.Where(t => t.Status == status.Value);
        }

        return filteredTasks.ToList();
    }

    public IEnumerable<Task> FilterByEmployeeId(
        IEnumerable<Task> tasks,
        long employeeId)
    {
        return tasks.Where(task =>
        {
            TaskMetadata? metadata = task.GetMetadata();
            return metadata?.EmployeeId == employeeId;
        });
    }

    public IEnumerable<Task> FilterByEmployeeRole(
        IEnumerable<Task> tasks,
        string employeeRole)
    {
        return tasks.Where(task =>
        {
            TaskMetadata? metadata = task.GetMetadata();
            return metadata?.EmployeeRole == employeeRole;
        });
    }
}
