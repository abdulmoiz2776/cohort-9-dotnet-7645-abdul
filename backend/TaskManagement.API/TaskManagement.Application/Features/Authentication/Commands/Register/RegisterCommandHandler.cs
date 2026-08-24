using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<RegisterResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Step 7 - Duplicate Email Validation
        var emailExists = await _userManager.FindByEmailAsync(request.Email);

        if (emailExists != null)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = "Email already exists."
            };
        }

        // Step 7 - Duplicate Username Validation
        var usernameExists = await _userManager.FindByNameAsync(request.Username);

        if (usernameExists != null)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = "Username already exists."
            };
        }

        // Step 8 - Create ApplicationUser
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
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Message = string.Join(", ", result.Errors.Select(x => x.Description))
            };
        }

        // Generate Email Confirmation Token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = Uri.EscapeDataString(token);

        var verificationUrl =
          $"https://localhost:5173/verify-email?userId={user.Id}&token={encodedToken}";

        Console.WriteLine("Generating email template...");
        // Read template
        var templatePath = Path.Combine(
            AppContext.BaseDirectory,
            "Email",
            "Templates",
            "EmailVerification.html");

        var html = await File.ReadAllTextAsync(
            templatePath,
            cancellationToken);
        Console.WriteLine($"Template Path: {templatePath}");
        Console.WriteLine($"Exists: {File.Exists(templatePath)}");

        html = html.Replace("{{UserName}}", user.FirstName);
        html = html.Replace("{{VerificationLink}}", verificationUrl);
        Console.WriteLine($"Sending email to {user.Email}");
        try
        {
            await _emailService.SendHtmlAsync(
                user.Email!,
                "Verify Your Email",
                html,
                cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine("========== EMAIL ERROR ==========");
            Console.WriteLine(ex.ToString());

            return new RegisterResponse
            {
                Succeeded = false,
                Message = ex.Message
            };
        }


        return new RegisterResponse
        {
            Succeeded = true,
            Message = "Registration completed successfully. Please verify your email.",
            UserId = user.Id,
            Email = user.Email,
            RequiresEmailVerification = true
        };
    }
}