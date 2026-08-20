
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Identity;
using TaskStatusEnum = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Features.Task.CreateTask;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, CreateTaskResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        ILogger<CreateTaskCommandHandler> logger)
    {
        _context = context;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CreateTaskResponse> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get authenticated user
        var currentUser = _currentUserService.User;

        if (currentUser is null ||
            string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new UnauthorizedAccessException(
                "Authenticated user could not be identified.");
        }

        var currentUserId = currentUser.UserId;

        // 2. Verify current user exists and is active
        var creator = await _userManager.FindByIdAsync(currentUserId);

        if (creator is null)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user was not found.");
        }

        if (!creator.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Your account is inactive.");
        }

        // 3. Validate category if supplied
        if (request.CategoryId.HasValue)
        {
            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(
                    x => x.Id == request.CategoryId.Value,
                    cancellationToken);

            if (category is null)
            {
                throw new KeyNotFoundException(
                    "Category not found.");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException(
                    "Category is inactive.");
            }
        }

        // 4. Validate assigned user if supplied
        var isAdmin = currentUser.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);
        var targetAssignedUserId = string.IsNullOrWhiteSpace(request.AssignedToUserId)
            ? null
            : request.AssignedToUserId;

        if (targetAssignedUserId is not null)
        {
            if (!isAdmin && !string.Equals(targetAssignedUserId, currentUserId, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("You can only assign tasks to yourself.");
            }

            var assignedUser = await _userManager.FindByIdAsync(targetAssignedUserId);

            if (assignedUser is null)
            {
                throw new KeyNotFoundException("Assigned user not found.");
            }

            if (!assignedUser.IsActive)
            {
                throw new InvalidOperationException("Assigned user is inactive.");
            }
        }

        // 5. Create TaskItem
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),

            Title = request.Title.Trim(),

            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),

            Status = TaskStatusEnum.Pending,

            Priority = request.Priority,

            CategoryId = request.CategoryId,

            AssignedToUserId = targetAssignedUserId,

            CreatedByUserId = currentUserId,

            DueDate = request.DueDate,

            CreatedAt = DateTime.UtcNow,

            UpdatedAt = null,

            CompletedAt = null,

            IsDeleted = false,

            DeletedAt = null
        };

        // 6. Save task
        _context.Add(task);

        await _context.SaveChangesAsync(cancellationToken);

        // 7. Log event
        _logger.LogInformation(
            "Task created successfully. TaskId: {TaskId}, CreatedByUserId: {CreatedByUserId}, AssignedToUserId: {AssignedToUserId}",
            task.Id,
            task.CreatedByUserId,
            task.AssignedToUserId);

        // 8. Return response
        return new CreateTaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Status = task.Status,
            Priority = task.Priority,
            CategoryId = task.CategoryId,
            AssignedToUserId = task.AssignedToUserId,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            CreatedByUserId = task.CreatedByUserId
        };
    }
}
