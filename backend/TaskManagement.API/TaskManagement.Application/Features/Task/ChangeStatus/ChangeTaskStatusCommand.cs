using MediatR;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Task.ChangeStatus;

public sealed class ChangeTaskStatusCommand : IRequest<ChangeTaskStatusResponse>
{
    public Guid Id { get; set; }

    public TaskStatus Status { get; set; }
}
