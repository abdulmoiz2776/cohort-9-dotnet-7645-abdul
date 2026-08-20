using MediatR;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Task.CreateTask;

public record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    Guid? CategoryId,
    string? AssignedToUserId,
    DateTime? DueDate
) : IRequest<CreateTaskResponse>;