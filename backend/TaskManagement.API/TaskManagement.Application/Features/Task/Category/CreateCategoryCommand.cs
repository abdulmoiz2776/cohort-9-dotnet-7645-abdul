using MediatR;

namespace TaskManagement.Application.Features.Task.Category;

public sealed class CreateCategoryCommand : IRequest<CreateCategoryResponse>
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
