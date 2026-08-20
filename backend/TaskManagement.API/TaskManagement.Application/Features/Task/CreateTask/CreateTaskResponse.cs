using TaskManagement.Domain.Enums;
using TaskStatusEnum = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Task.CreateTask;

public class CreateTaskResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public TaskStatusEnum Status { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid? CategoryId { get; set; }

    public string? AssignedToUserId { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;
}