using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Application.Features.Task.AssignTask;

public sealed class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, AssignTaskResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AssignTaskCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AssignTaskResponse> Handle(
        AssignTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        var assignedUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == request.AssignedToUserId, cancellationToken);

        if (assignedUser is null || !assignedUser.IsActive)
        {
            return new AssignTaskResponse
            {
                Succeeded = false,
                Message = "Assigned user not found or is not active."
            };
        }

        var currentUser = _currentUserService.User;
        var isAdmin = currentUser.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);

        if (!isAdmin && string.IsNullOrWhiteSpace(currentUser.UserId))
            throw new UnauthorizedAccessException("User must be authenticated.");

        if (!isAdmin && task.CreatedByUserId != currentUser.UserId)
            throw new UnauthorizedAccessException("You are not authorized to assign this task.");

        task.AssignedToUserId = request.AssignedToUserId;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new AssignTaskResponse
        {
            Succeeded = true,
            Message = "Task assigned successfully."
        };
    }
}
