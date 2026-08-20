
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Identity;
using TaskStatusEnum = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public Guid? CategoryId { get; set; }

    public string? AssignedToUserId { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public TaskCategory? Category { get; set; }

    public ApplicationUser? AssignedToUser { get; set; }

    public ApplicationUser CreatedByUser { get; set; } = null!;
}
