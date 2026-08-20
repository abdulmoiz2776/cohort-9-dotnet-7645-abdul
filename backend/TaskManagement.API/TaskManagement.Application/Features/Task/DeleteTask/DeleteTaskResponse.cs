namespace TaskManagement.Application.Features.Task.DeleteTask;

public sealed class DeleteTaskResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}
