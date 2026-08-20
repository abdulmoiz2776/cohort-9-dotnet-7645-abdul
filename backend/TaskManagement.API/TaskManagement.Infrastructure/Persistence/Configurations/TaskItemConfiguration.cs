 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        // =========================================================
        // Table
        // =========================================================

        builder.ToTable("Tasks");

        // =========================================================
        // Primary Key
        // =========================================================

        builder.HasKey(x => x.Id);

        // =========================================================
        // Properties
        // =========================================================

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnType("nvarchar(max)");

        // =========================================================
        // Enum Storage
        // =========================================================

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<int>()
            .IsRequired();

        // =========================================================
        // Required Properties
        // =========================================================

        builder.Property(x => x.CreatedByUserId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        // =========================================================
        // Category Relationship
        // CategoryId is optional
        // =========================================================

        builder.HasOne(x => x.Category)
            .WithMany(x => x.TaskItems)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // Assigned User Relationship
        // AssignedToUserId is optional
        // =========================================================

        builder.HasOne(x => x.AssignedToUser)
            .WithMany(x => x.AssignedTasks)
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // =========================================================
        // Created By User Relationship
        // CreatedByUserId is required
        // =========================================================

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedTasks)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // Indexes
        // =========================================================

        builder.HasIndex(x => x.CategoryId);

        builder.HasIndex(x => x.AssignedToUserId);

        builder.HasIndex(x => x.CreatedByUserId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);

        builder.HasIndex(x => x.DueDate);

        builder.HasIndex(x => x.IsDeleted);

        // =========================================================
        // Composite Indexes
        // =========================================================

        builder.HasIndex(x => new
        {
            x.AssignedToUserId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.AssignedToUserId,
            x.DueDate
        });
    }
}
