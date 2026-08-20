namespace TaskManagement.Application.Features.Task.Category;

public sealed class CreateCategoryResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public Guid? CategoryId { get; set; }
}
