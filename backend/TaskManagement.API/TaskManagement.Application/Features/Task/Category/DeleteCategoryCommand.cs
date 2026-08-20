using MediatR;

namespace TaskManagement.Application.Features.Task.Category;

public sealed class DeleteCategoryCommand : IRequest<DeleteCategoryResponse>
{
    public Guid Id { get; set; }
}
