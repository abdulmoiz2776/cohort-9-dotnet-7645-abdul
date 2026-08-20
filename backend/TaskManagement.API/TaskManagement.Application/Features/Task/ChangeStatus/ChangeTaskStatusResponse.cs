namespace TaskManagement.Application.Features.Task.ChangeStatus;

public sealed class ChangeTaskStatusResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}
