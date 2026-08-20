using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.ResendVerificationEmail;

public class ResendVerificationEmailCommandHandler
    : IRequestHandler<
        ResendVerificationEmailCommand,
        ResendVerificationEmailResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public ResendVerificationEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<ResendVerificationEmailResponse> Handle(
        ResendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return new ResendVerificationEmailResponse
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        if (user.EmailConfirmed)
        {
            return new ResendVerificationEmailResponse
            {
                Succeeded = false,
                Message = "Email is already verified."
            };
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = Uri.EscapeDataString(token);

        var verificationLink =
            $"http://localhost:5173/verify-email?userId={user.Id}&token={encodedToken}";

        var templatePath = Path.Combine(
            AppContext.BaseDirectory,
            "Email",
            "Templates",
            "EmailVerification.html");

        var html = await File.ReadAllTextAsync(
            templatePath,
            cancellationToken);

        html = html.Replace("{{UserName}}", user.FirstName);
        html = html.Replace("{{VerificationLink}}", verificationLink);

        await _emailService.SendHtmlAsync(
            user.Email!,
            "Verify Your Email",
            html,
            cancellationToken);

        return new ResendVerificationEmailResponse
        {
            Succeeded = true,
            Message = "Verification email sent successfully."
        };
    }
}