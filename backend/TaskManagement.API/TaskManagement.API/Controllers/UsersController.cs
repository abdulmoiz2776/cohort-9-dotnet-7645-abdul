using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Models;
using TaskManagement.Domain.Identity;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentProfile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return NotFound(new { Message = "User not found." });

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            Roles = roles.ToArray()
        });
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return NotFound(new { Message = "User not found." });

        if (!string.IsNullOrWhiteSpace(request.Username) &&
            !string.Equals(user.UserName, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            var existingUsername = await _userManager.FindByNameAsync(request.Username);
            if (existingUsername is not null && existingUsername.Id != user.Id)
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            user.UserName = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName;

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new { Message = string.Join("; ", result.Errors.Select(x => x.Description)) });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            Roles = roles.ToArray()
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                Username = u.UserName ?? string.Empty,
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailConfirmed = u.EmailConfirmed,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = Array.Empty<string>()
            })
            .ToListAsync();

        var userRoleTasks = users.Select(async dto =>
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user != null)
            {
                dto.Roles = (await _userManager.GetRolesAsync(user)).ToArray();
            }
            return dto;
        });

        var enrichedUsers = await Task.WhenAll(userRoleTasks);

        return Ok(enrichedUsers);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound(new { Message = "User not found." });

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToArray()
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles
            .Select(r => new RoleDto
            {
                Name = r.Name ?? string.Empty
            })
            .ToList();

        return Ok(roles);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRole(string id, AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound(new { Message = "User not found." });

        if (string.IsNullOrWhiteSpace(request.RoleName))
            return BadRequest(new { Message = "RoleName is required." });

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
            return BadRequest(new { Message = "Role does not exist." });

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
            return BadRequest(new { Message = string.Join("; ", result.Errors.Select(x => x.Description)) });

        return Ok(new { Message = "Role assigned." });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/roles/{roleName}")]
    public async Task<IActionResult> RemoveRole(string id, string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound(new { Message = "User not found." });

        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest(new { Message = "RoleName is required." });

        if (!await _roleManager.RoleExistsAsync(roleName))
            return BadRequest(new { Message = "Role does not exist." });

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
            return BadRequest(new { Message = string.Join("; ", result.Errors.Select(x => x.Description)) });

        return Ok(new { Message = "Role removed." });
    }

    public sealed record UpdateProfileRequest(
        string FirstName,
        string LastName,
        string Username);

    public sealed record AssignRoleRequest(string RoleName);
}
