using TaskPriority = TaskManagement.Domain.Enums.TaskPriority;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Task.Common;

public sealed class TaskDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public TaskStatus Status { get; init; }

    public TaskPriority Priority { get; init; }

    public Guid? CategoryId { get; init; }

    public string? CategoryName { get; init; }

    public string? AssignedToUserId { get; init; }

    public string? AssignedToUsername { get; init; }

    public string CreatedByUserId { get; init; } = string.Empty;

    public DateTime? DueDate { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public DateTime? CompletedAt { get; init; }
}
