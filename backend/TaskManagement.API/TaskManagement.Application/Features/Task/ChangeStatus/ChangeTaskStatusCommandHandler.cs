using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Task.ChangeStatus;

public sealed class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand, ChangeTaskStatusResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ChangeTaskStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ChangeTaskStatusResponse> Handle(
        ChangeTaskStatusCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        var currentUser = _currentUserService.User;
        var isAdmin = currentUser.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);

        if (!isAdmin && string.IsNullOrWhiteSpace(currentUser.UserId))
            throw new UnauthorizedAccessException("User must be authenticated.");

        var isOwner = task.CreatedByUserId == currentUser.UserId;
        var isAssignee = task.AssignedToUserId == currentUser.UserId;

        if (!isAdmin && !isOwner && !isAssignee)
        {
            throw new UnauthorizedAccessException("You are not authorized to change task status.");
        }

        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;
        task.CompletedAt = request.Status == TaskStatus.Completed
            ? DateTime.UtcNow
            : null;

        await _context.SaveChangesAsync(cancellationToken);

        return new ChangeTaskStatusResponse
        {
            Succeeded = true,
            Message = "Task status updated successfully."
        };
    }
}
