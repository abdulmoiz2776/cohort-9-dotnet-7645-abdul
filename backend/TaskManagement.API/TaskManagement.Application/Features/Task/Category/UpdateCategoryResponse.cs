namespace TaskManagement.Application.Features.Task.Category;

public sealed class UpdateCategoryResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}
