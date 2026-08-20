using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Task.UpdateTask;

public sealed class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, UpdateTaskResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTaskCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UpdateTaskResponse> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (task is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        var currentUser = _currentUserService.User;
        var isAdmin = currentUser.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);

        if (!isAdmin && string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User must be authenticated.");
        }

        if (!isAdmin && task.CreatedByUserId != currentUser.UserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this task.");
        }

        if (request.CategoryId.HasValue)
        {
            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId.Value, cancellationToken);

            if (category is null || !category.IsActive)
            {
                return new UpdateTaskResponse
                {
                    Succeeded = false,
                    Message = "Category not found or is inactive."
                };
            }
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.CategoryId = request.CategoryId;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateTaskResponse
        {
            Succeeded = true,
            Message = "Task updated successfully."
        };
    }
}
