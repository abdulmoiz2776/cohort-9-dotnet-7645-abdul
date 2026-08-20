using MediatR;

namespace TaskManagement.Application.Features.Task.DeleteTask;

public sealed class DeleteTaskCommand : IRequest<DeleteTaskResponse>
{
    public Guid Id { get; set; }
}
