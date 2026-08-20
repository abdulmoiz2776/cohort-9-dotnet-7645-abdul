using MediatR;

namespace TaskManagement.Application.Features.Task.AssignTask;

public sealed class AssignTaskCommand : IRequest<AssignTaskResponse>
{
    public Guid Id { get; set; }

    public string AssignedToUserId { get; set; } = string.Empty;
}
