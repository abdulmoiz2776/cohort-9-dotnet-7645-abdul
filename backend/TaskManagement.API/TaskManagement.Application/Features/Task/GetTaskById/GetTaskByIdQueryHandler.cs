using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Task.Common;

namespace TaskManagement.Application.Features.Task.GetTaskById;

public sealed class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskDetailsDto> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .Include(x => x.Category)
            .Include(x => x.AssignedToUser)
            .Include(x => x.CreatedByUser)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (task is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        var currentUser = _currentUserService.User;
        var isAdmin = currentUser.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);

        if (!isAdmin && currentUser.UserId is not null)
        {
            var isOwner = task.CreatedByUserId == currentUser.UserId;
            var isAssignee = task.AssignedToUserId == currentUser.UserId;
            if (!isOwner && !isAssignee)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this task.");
            }
        }

        if (!isAdmin && string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User must be authenticated.");
        }

        return new TaskDetailsDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            CategoryId = task.CategoryId,
            CategoryName = task.Category?.Name,
            CategoryDescription = task.Category?.Description,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUsername = task.AssignedToUser?.UserName,
            CreatedByUserId = task.CreatedByUserId,
            CreatedByUsername = task.CreatedByUser?.UserName,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            CompletedAt = task.CompletedAt
        };
    }
}
