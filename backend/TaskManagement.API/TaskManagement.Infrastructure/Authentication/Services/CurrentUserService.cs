using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.Infrastructure.Authentication.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser User
    {
        get
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null || ctx.User == null)
            {
                return new CurrentUser { IsAuthenticated = false };
            }

            var user = ctx.User;

            var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            return new CurrentUser
            {
                UserId = userId,
                Email = email,
                Username = username,
                IsAuthenticated = isAuthenticated,
                Roles = roles
            };
        }
    }
}

