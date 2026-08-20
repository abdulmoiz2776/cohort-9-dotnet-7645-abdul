
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IApplicationUrlService _applicationUrlService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IApplicationUrlService applicationUrlService,
        ILogger<RegisterCommandHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _applicationUrlService = applicationUrlService;
        _logger = logger;
    }

    public async Task<RegisterResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1 - Validate request
        if (request is null)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = "Registration request is required."
            };
        }

        // Step 2 - Validate email
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = "Email is required."
            };
        }

        // Step 3 - Duplicate Email Validation
        var emailExists =
            await _userManager.FindByEmailAsync(request.Email);

        if (emailExists != null)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = "Email already exists."
            };
        }

        // Step 4 - Duplicate Username Validation
        var usernameExists =
            await _userManager.FindByNameAsync(request.Username);

        if (usernameExists != null)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = "Username already exists."
            };
        }

        // Step 5 - Create ApplicationUser
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Username,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Password hashing is handled automatically by ASP.NET Identity
        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description))
            };
        }

        try
        {
            // Step 6 - Generate Email Confirmation Token
            var token =
                await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken =
                WebUtility.UrlEncode(token);

            // Step 7 - Build Verification URL
            var verificationUrl =
                $"{_applicationUrlService.FrontendBaseUrl}/verify-email" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={encodedToken}";

            // Step 8 - Read Email Template
            var templatePath = Path.Combine(
                AppContext.BaseDirectory,
                "Email",
                "Templates",
                "EmailVerification.html");

            var html = await File.ReadAllTextAsync(
                templatePath,
                cancellationToken);

            // Step 9 - HTML Encode Template Values
            html = html.Replace(
                "{{UserName}}",
                WebUtility.HtmlEncode(user.FirstName));

            html = html.Replace(
                "{{VerificationLink}}",
                WebUtility.HtmlEncode(verificationUrl));

            // Step 10 - Send Verification Email
            await _emailService.SendHtmlAsync(
                user.Email,
                "Verify Your Email",
                html,
                cancellationToken);

            _logger.LogInformation(
                "Verification email sent successfully. UserId: {UserId}",
                user.Id);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Verification email operation cancelled. UserId: {UserId}",
                user.Id);

            throw;
        }
        catch (Exception ex)
        {
            // User has already been created successfully.
            // Do not report registration as failed.
            _logger.LogError(
                ex,
                "User registered successfully, but verification email could not be sent. UserId: {UserId}",
                user.Id);
        }

        // Step 11 - Registration Success
        return new RegisterResponse
        {
            Succeeded = true,
            Message =
                "Registration completed successfully. Please verify your email.",
            UserId = user.Id,
            Email = user.Email,
            RequiresEmailVerification = true
        };
    }
}

