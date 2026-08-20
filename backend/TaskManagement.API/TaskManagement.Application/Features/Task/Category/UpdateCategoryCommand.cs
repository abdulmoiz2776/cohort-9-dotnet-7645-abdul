using MediatR;

namespace TaskManagement.Application.Features.Task.Category;

public sealed class UpdateCategoryCommand : IRequest<UpdateCategoryResponse>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
