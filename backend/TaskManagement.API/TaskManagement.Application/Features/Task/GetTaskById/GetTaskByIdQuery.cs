using MediatR;
using TaskManagement.Application.Features.Task.Common;

namespace TaskManagement.Application.Features.Task.GetTaskById;

public sealed class GetTaskByIdQuery : IRequest<TaskDetailsDto>
{
    public Guid Id { get; set; }
}
