using Microsoft.EntityFrameworkCore;
using Task = TaskServiceDomain.Models.Task;

namespace TaskServiceInfrastructure.Infrastructure.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    public DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Deadline).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.MetadataJson).HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.OwnsMany(
                e => e.History,
                history =>
                {
                    history.Property(h => h.UserId).IsRequired();
                    history.Property(h => h.HistoryType).IsRequired();
                    history.Property(h => h.Timestamp).IsRequired();
                });
        });
    }
}