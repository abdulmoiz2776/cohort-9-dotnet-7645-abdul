using MediatR;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Task.UpdateTask;

public sealed class UpdateTaskCommand : IRequest<UpdateTaskResponse>
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid? CategoryId { get; set; }

    public DateTime? DueDate { get; set; }
}
