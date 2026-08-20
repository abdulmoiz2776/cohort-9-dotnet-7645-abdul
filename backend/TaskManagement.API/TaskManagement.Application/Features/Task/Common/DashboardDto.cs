namespace TaskManagement.Application.Features.Task.Common;

public sealed class DashboardDto
{
    public int TotalTasks { get; init; }

    public int PendingCount { get; init; }

    public int InProgressCount { get; init; }

    public int CompletedCount { get; init; }
}
