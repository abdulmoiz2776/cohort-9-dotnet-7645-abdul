using MediatR;
using TaskManagement.Application.Common.Models;
using TaskPriority = TaskManagement.Domain.Enums.TaskPriority;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Task.GetTasks;

public sealed class GetTasksQuery : IRequest<PagedResult<TaskManagement.Application.Features.Task.Common.TaskDto>>
{
    public string? SearchTerm { get; set; }

    public TaskStatus? Status { get; set; }

    public TaskPriority? Priority { get; set; }

    public Guid? CategoryId { get; set; }

    public string? AssignedToUserId { get; set; }

    public DateTime? DueDate { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string SortBy { get; set; } = "CreatedAt";

    public bool SortDescending { get; set; }
}
