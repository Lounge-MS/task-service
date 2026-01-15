using Microsoft.EntityFrameworkCore;
using TaskServiceDomain.Interfaces;
using TaskServiceInfrastructure.Infrastructure.Data;
using Task = TaskServiceDomain.Models.Task;

namespace TaskServiceInfrastructure.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<Task?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Include(t => t.History)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Task> CreateAsync(Task task, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<Task> UpdateAsync(Task task, CancellationToken cancellationToken = default)
    {
        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        Task? task = await GetByIdAsync(id, cancellationToken);
        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<Task>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Include(t => t.History)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Task>> GetByStatusAsync(
        TaskServiceDomain.Models.TaskStatus status,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .Include(t => t.History)
            .Where(t => t.Status == status)
            .ToListAsync(cancellationToken);
    }
}