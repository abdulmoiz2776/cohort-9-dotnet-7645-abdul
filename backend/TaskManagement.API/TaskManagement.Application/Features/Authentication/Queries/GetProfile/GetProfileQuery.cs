using MediatR;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.Application.Features.Authentication.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<UserProfileDto>;
