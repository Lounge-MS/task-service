using Grpc.Core;
using TaskService.Grpc;
using TaskServiceDomain.Interfaces;
using TaskServiceDomain.Models;
using TaskServiceGrpc.Mappers;

namespace TaskServiceGrpc.Controllers;

public class TaskServiceController : TaskService.Grpc.TaskService.TaskServiceBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskServiceController> _logger;

    public TaskServiceController(ITaskService taskService, ILogger<TaskServiceController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    public override async Task<TaskResponse> CreateTask(CreateTaskRequest request, ServerCallContext context)
    {
        try
        {
            var deadline = new DateTime(request.DeadlineTicks, DateTimeKind.Utc);
            TaskServiceDomain.Models.TaskMetadata? metadata = TaskMapper.ToDomainTaskMetadata(request.Metadata);

            TaskServiceDomain.Models.Task task = await _taskService.CreateTaskAsync(
                request.Title,
                request.Description,
                deadline,
                request.CreatedByUserId,
                metadata,
                context.CancellationToken);

            return new TaskResponse
            {
                Success = true,
                Task = TaskMapper.ToDto(task),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return new TaskResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public override async Task<TaskResponse> CancelTask(CancelTaskRequest request, ServerCallContext context)
    {
        try
        {
            TaskServiceDomain.Models.Task? task = await _taskService.CancelTaskAsync(
                request.TaskId,
                request.CanceledByUserId,
                context.CancellationToken);

            if (task == null)
            {
                return new TaskResponse
                {
                    Success = false,
                    ErrorMessage = "Task not found",
                };
            }

            return new TaskResponse
            {
                Success = true,
                Task = TaskMapper.ToDto(task),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling task");
            return new TaskResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public override async Task<TaskResponse> CompleteTask(CompleteTaskRequest request, ServerCallContext context)
    {
        try
        {
            TaskServiceDomain.Models.Task? task = await _taskService.CompleteTaskAsync(
                request.TaskId,
                request.CompletedByUserId,
                context.CancellationToken);

            if (task == null)
            {
                return new TaskResponse
                {
                    Success = false,
                    ErrorMessage = "Task not found",
                };
            }

            return new TaskResponse
            {
                Success = true,
                Task = TaskMapper.ToDto(task),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task");
            return new TaskResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public override async Task<TaskResponse> UpdateTaskStatus(UpdateTaskStatusRequest request, ServerCallContext context)
    {
        try
        {
            TaskServiceDomain.Models.TaskStatus newStatus = TaskMapper.ToDomainTaskStatus(request.NewStatus);
            TaskServiceDomain.Models.Task? task = await _taskService.UpdateTaskStatusAsync(
                request.TaskId,
                newStatus,
                request.UpdatedByUserId,
                context.CancellationToken);

            if (task == null)
            {
                return new TaskResponse
                {
                    Success = false,
                    ErrorMessage = "Task not found",
                };
            }

            return new TaskResponse
            {
                Success = true,
                Task = TaskMapper.ToDto(task),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task status");
            return new TaskResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public override async Task<TaskResponse> GetTaskById(GetTaskByIdRequest request, ServerCallContext context)
    {
        try
        {
            TaskServiceDomain.Models.Task? task = await _taskService.GetTaskByIdAsync(request.TaskId, context.CancellationToken);

            if (task == null)
            {
                return new TaskResponse
                {
                    Success = false,
                    ErrorMessage = "Task not found",
                };
            }

            return new TaskResponse
            {
                Success = true,
                Task = TaskMapper.ToDto(task),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task by id");
            return new TaskResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public override async Task<GetTasksResponse> GetTasksWithFilters(GetTasksWithFiltersRequest request, ServerCallContext context)
    {
        try
        {
            TaskServiceDomain.Models.TaskStatus? status = null;

            if (request.Status != TaskService.Grpc.TaskStatus.Unspecified)
            {
                status = TaskMapper.ToDomainTaskStatus(request.Status);
            }

            IEnumerable<TaskServiceDomain.Models.Task?> tasks = await _taskService.GetTasksWithFiltersAsync(status, context.CancellationToken);

            var response = new GetTasksResponse
            {
                Success = true,
            };

            response.Tasks.AddRange(tasks.Select(TaskMapper.ToDto));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks with filters");
            return new GetTasksResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public override async Task<GetTaskHistoryResponse> GetTaskHistory(GetTaskHistoryRequest request, ServerCallContext context)
    {
        try
        {
            IEnumerable<TaskHistory> history = await _taskService.GetTaskHistoryAsync(request.TaskId, context.CancellationToken);

            var response = new GetTaskHistoryResponse
            {
                Success = true,
            };

            response.History.AddRange(history.Select(TaskMapper.ToDto));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task history");
            return new GetTaskHistoryResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }
}
