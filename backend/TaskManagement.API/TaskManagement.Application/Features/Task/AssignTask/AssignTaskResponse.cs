namespace TaskManagement.Application.Features.Task.AssignTask;

public sealed class AssignTaskResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}
