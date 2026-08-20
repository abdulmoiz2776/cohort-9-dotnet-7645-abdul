using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Task.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Task.GetTasks;

public sealed class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PagedResult<TaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTasksQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<TaskDto>> Handle(
        GetTasksQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.User;

        var query = _context.TaskItems
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        var isAdmin = currentUser.Roles
            .Contains("Admin", StringComparer.OrdinalIgnoreCase);

        if (!isAdmin)
        {
            if (string.IsNullOrWhiteSpace(currentUser.UserId))
            {
                throw new UnauthorizedAccessException("User must be authenticated.");
            }

            query = query.Where(x => x.CreatedByUserId == currentUser.UserId ||
                                     x.AssignedToUserId == currentUser.UserId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(x => x.Title.Contains(searchTerm) ||
                                     (x.Description ?? string.Empty).Contains(searchTerm));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(x => x.Priority == request.Priority.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.AssignedToUserId))
        {
            query = query.Where(x => x.AssignedToUserId == request.AssignedToUserId);
        }

        if (request.DueDate.HasValue)
        {
            var targetDate = request.DueDate.Value.Date;
            query = query.Where(x => x.DueDate.HasValue && x.DueDate.Value.Date == targetDate);
        }

        query = query.Include(x => x.Category)
                     .Include(x => x.AssignedToUser);

        query = ApplySorting(query, request.SortBy, request.SortDescending);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((Math.Max(request.Page, 1) - 1) * Math.Max(request.PageSize, 1))
            .Take(Math.Max(request.PageSize, 1))
            .Select(x => new TaskDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                Priority = x.Priority,
                CategoryId = x.CategoryId,
                CategoryName = x.Category != null ? x.Category.Name : null,
                AssignedToUserId = x.AssignedToUserId,
                AssignedToUsername = x.AssignedToUser != null ? x.AssignedToUser.UserName : null,
                CreatedByUserId = x.CreatedByUserId,
                DueDate = x.DueDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                CompletedAt = x.CompletedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskDto>
        {
            Page = Math.Max(request.Page, 1),
            PageSize = Math.Max(request.PageSize, 1),
            TotalCount = totalCount,
            Items = items
        };
    }

    private static IQueryable<TaskItem> ApplySorting(
        IQueryable<TaskItem> query,
        string sortBy,
        bool sortDescending)
    {
        Expression<Func<TaskItem, object>> sortExpression = sortBy?.ToLowerInvariant() switch
        {
            "title" => x => x.Title,
            "priority" => x => x.Priority,
            "status" => x => x.Status,
            "duedate" => x => x.DueDate,
            "updatedat" => x => x.UpdatedAt,
            _ => x => x.CreatedAt
        };

        return sortDescending 
            ? query.OrderByDescending(sortExpression)
            : query.OrderBy(sortExpression);
    }
}
