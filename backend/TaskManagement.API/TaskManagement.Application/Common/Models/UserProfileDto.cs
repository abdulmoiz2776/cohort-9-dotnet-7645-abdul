namespace TaskManagement.Application.Common.Models;

public sealed class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
}
