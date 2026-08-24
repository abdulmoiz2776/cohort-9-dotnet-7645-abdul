using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.Features.Authentication.Commands.Logout;
using TaskManagement.Application.Features.Authentication.Commands.LogoutAll;
using TaskManagement.Application.Features.Authentication.Commands.Register;
using TaskManagement.Application.Features.Authentication.Login;
using TaskManagement.Application.Features.Authentication.ResendVerificationEmail;
using TaskManagement.Application.Features.Authentication.VerifyEmail;
namespace TaskManagement.API.Controllers;

using TaskManagement.Application.Features.Authentication.Commands.ForgotPassword;
using TaskManagement.Application.Features.Authentication.Commands.RevokeSession;
using TaskManagement.Application.Features.Authentication.Queries.GetSessions;
using TaskManagement.Application.Features.Authentication.Commands.RefreshToken;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(
    [FromQuery] string userId,
    [FromQuery] string token)
    {
        var result = await _mediator.Send(
            new VerifyEmailCommand
            {
                UserId = userId,
                Token = token
            });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(
    ResendVerificationEmailCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _mediator.Send(new LogoutAllCommand
        {
            UserId = userId
        });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    [Authorize]
    [HttpGet("sessions")]
    public async Task<ActionResult<List<SessionDto>>> GetSessions()
    {
        var result = await _mediator.Send(new GetSessionsQuery());

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("sessions/{sessionId:guid}")]
    public async Task<ActionResult<RevokeSessionResponse>> RevokeSession(Guid sessionId)
    {
        return Ok(await _mediator.Send(
            new RevokeSessionCommand(sessionId)));
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(
    ForgotPasswordCommand command)
    {
        var response =
            await _mediator.Send(command);

        return Ok(response);
    }
    [HttpPost("reset-password")]
    public async Task<ActionResult<ResetPasswordResponse>>
    ResetPassword(
        ResetPasswordCommand command)
    {
        var result =
            await _mediator.Send(command);

        return Ok(result);
    }
    [Authorize]

    [HttpPost("change-password")]
    public async Task<ActionResult<ChangePasswordResponse>>
    ChangePassword(
        ChangePasswordCommand command)
    {
        var result =
            await _mediator.Send(command);

        return Ok(result);
    }
}