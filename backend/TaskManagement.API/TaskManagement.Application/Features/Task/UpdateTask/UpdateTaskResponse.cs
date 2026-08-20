namespace TaskManagement.Application.Features.Task.UpdateTask;

public sealed class UpdateTaskResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}
