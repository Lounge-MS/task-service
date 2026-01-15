using TaskService.Grpc;
using TaskServiceDomain.Models;

namespace TaskServiceGrpc.Mappers;

public static class TaskMapper
{
    public static TaskDto ToDto(TaskServiceDomain.Models.Task? task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var dto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DeadlineTicks = task.Deadline.Ticks,
            Status = ToProtoTaskStatus(task.Status),
            CreatedAtTicks = task.CreatedAt.Ticks,
            UpdatedAtTicks = task.UpdatedAt.Ticks,
        };

        TaskServiceDomain.Models.TaskMetadata? metadata = task.GetMetadata();
        if (metadata != null)
        {
            dto.Metadata = new TaskService.Grpc.TaskMetadata();
            if (metadata.EmployeeId.HasValue)
            {
                dto.Metadata.EmployeeId = metadata.EmployeeId.Value;
            }

            if (!string.IsNullOrEmpty(metadata.EmployeeRole))
            {
                dto.Metadata.EmployeeRole = metadata.EmployeeRole;
            }
        }

        return dto;
    }

    public static TaskHistoryDto ToDto(TaskHistory history)
    {
        return new TaskHistoryDto
        {
            UserId = history.UserId,
            HistoryType = ToProtoHistoryType(history.HistoryType),
            TimestampTicks = history.Timestamp.Ticks,
        };
    }

    public static TaskServiceDomain.Models.TaskStatus ToDomainTaskStatus(TaskService.Grpc.TaskStatus protoStatus)
    {
        return protoStatus switch
        {
            TaskService.Grpc.TaskStatus.Actual => TaskServiceDomain.Models.TaskStatus.ACTUAL,
            TaskService.Grpc.TaskStatus.Completed => TaskServiceDomain.Models.TaskStatus.COMPLETED,
            TaskService.Grpc.TaskStatus.Canceled => TaskServiceDomain.Models.TaskStatus.CANCELED,
            TaskService.Grpc.TaskStatus.Unspecified => TaskServiceDomain.Models.TaskStatus.ACTUAL,
            _ => TaskServiceDomain.Models.TaskStatus.ACTUAL,
        };
    }

    public static TaskService.Grpc.TaskStatus ToProtoTaskStatus(TaskServiceDomain.Models.TaskStatus domainStatus)
    {
        return domainStatus switch
        {
            TaskServiceDomain.Models.TaskStatus.ACTUAL => TaskService.Grpc.TaskStatus.Actual,
            TaskServiceDomain.Models.TaskStatus.COMPLETED => TaskService.Grpc.TaskStatus.Completed,
            TaskServiceDomain.Models.TaskStatus.CANCELED => TaskService.Grpc.TaskStatus.Canceled,
            _ => TaskService.Grpc.TaskStatus.Actual,
        };
    }

    public static TaskServiceDomain.Models.HistoryType ToDomainHistoryType(TaskService.Grpc.HistoryType protoType)
    {
        return protoType switch
        {
            TaskService.Grpc.HistoryType.Created => TaskServiceDomain.Models.HistoryType.CREATED,
            TaskService.Grpc.HistoryType.Completed => TaskServiceDomain.Models.HistoryType.COMPLETED,
            TaskService.Grpc.HistoryType.Canceled => TaskServiceDomain.Models.HistoryType.CANCELED,
            TaskService.Grpc.HistoryType.Unspecified => TaskServiceDomain.Models.HistoryType.CANCELED,
            _ => TaskServiceDomain.Models.HistoryType.CREATED,
        };
    }

    public static TaskService.Grpc.HistoryType ToProtoHistoryType(TaskServiceDomain.Models.HistoryType domainType)
    {
        return domainType switch
        {
            TaskServiceDomain.Models.HistoryType.CREATED => TaskService.Grpc.HistoryType.Created,
            TaskServiceDomain.Models.HistoryType.COMPLETED => TaskService.Grpc.HistoryType.Completed,
            TaskServiceDomain.Models.HistoryType.CANCELED => TaskService.Grpc.HistoryType.Canceled,
            _ => TaskService.Grpc.HistoryType.Created,
        };
    }

    public static TaskServiceDomain.Models.TaskMetadata? ToDomainTaskMetadata(
        TaskService.Grpc.TaskMetadata? protoMetadata)
    {
        if (protoMetadata == null)
            return null;

        var metadata = new TaskServiceDomain.Models.TaskMetadata();

        if (protoMetadata.EmployeeId > 0)
        {
            metadata.EmployeeId = protoMetadata.EmployeeId;
        }

        if (!string.IsNullOrEmpty(protoMetadata.EmployeeRole))
        {
            metadata.EmployeeRole = protoMetadata.EmployeeRole;
        }

        return metadata;
    }
}