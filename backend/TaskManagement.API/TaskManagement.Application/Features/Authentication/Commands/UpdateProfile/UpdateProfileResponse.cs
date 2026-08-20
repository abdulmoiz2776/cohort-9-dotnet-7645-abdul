namespace TaskManagement.Application.Features.Authentication.Commands.UpdateProfile;

public sealed class UpdateProfileResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}
