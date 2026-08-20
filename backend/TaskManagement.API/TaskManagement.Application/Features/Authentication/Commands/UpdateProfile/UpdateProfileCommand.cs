using MediatR;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.Application.Features.Authentication.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string Username)
    : IRequest<UpdateProfileResponse>;
