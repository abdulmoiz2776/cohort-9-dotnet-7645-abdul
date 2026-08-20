using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISessionService _sessionService;
    private readonly IClientInfoService _clientInfoService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        ISessionService sessionService,
        IClientInfoService clientInfoService,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _sessionService = sessionService;
        _clientInfoService = clientInfoService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Find user
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                _logger.LogWarning(
                    "Invalid login attempt for email {Email}",
                    request.Email);

                return new LoginResponse
                {
                    Succeeded = false,
                    Message = "Invalid email or password."
                };
            }

            // Check account active
            if (!user.IsActive)
            {
                _logger.LogWarning(
                    "Inactive account login attempt: {Email}",
                    request.Email);

                return new LoginResponse
                {
                    Succeeded = false,
                    Message = "Your account is inactive."
                };
            }

            // Check account locked
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning(
                    "Locked account login attempt: {Email}",
                    request.Email);

                return new LoginResponse
                {
                    Succeeded = false,
                    Message = "Your account is locked. Please try again later."
                };
            }

            // Check email confirmation
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning(
                    "Email not verified: {Email}",
                    request.Email);

                return new LoginResponse
                {
                    Succeeded = false,
                    Message = "Please verify your email before logging in."
                };
            }

            // Validate password
            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                _logger.LogWarning(
                    "User locked after failed login attempts: {Email}",
                    request.Email);

                return new LoginResponse
                {
                    Succeeded = false,
                    Message = "Your account has been locked. Please try again later."
                };
            }

            //if (!signInResult.Succeeded)
            //{
            //    _logger.LogWarning(
            //        "Invalid password for {Email}",
            //        request.Email);

            //    return new LoginResponse
            //    {
            //        Succeeded = false,
            //        Message = "Invalid email or password."
            //    };
            //}
            if (!signInResult.Succeeded)
            {
                _logger.LogWarning(
                    "Login failed for {Email}. Succeeded={Succeeded}, IsNotAllowed={IsNotAllowed}, IsLockedOut={IsLockedOut}, RequiresTwoFactor={RequiresTwoFactor}",
                    request.Email,
                    signInResult.Succeeded,
                    signInResult.IsNotAllowed,
                    signInResult.IsLockedOut,
                    signInResult.RequiresTwoFactor);

                return new LoginResponse
                {
                    Succeeded = false,
                    Message = "Invalid email or password."
                };
            }

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate Access Token
            var accessToken = await _jwtService.GenerateAccessTokenAsync(
                user.Id,
                user.UserName!,
                user.Email!,
                roles);

            // Client Information
            var client = _clientInfoService.GetSessionInfo();

            // Generate Refresh Token
            var refreshResult = await _refreshTokenService.GenerateAsync(
                user,
                client.IpAddress ?? "Unknown",
                request.RememberMe);

            // Create Session
            var session = new UserSession
            {
                ApplicationUserId = user.Id,
                RefreshTokenId = refreshResult.RefreshTokenEntity.Id,

                DeviceName = client.DeviceName,
                Browser = client.Browser,
                OperatingSystem = client.OperatingSystem,
                IpAddress = client.IpAddress,
                UserAgent = client.UserAgent,

                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow,
                ExpiresAt = refreshResult.RefreshTokenEntity.ExpiresAt,
                IsActive = true
            };

            await _sessionService.CreateAsync(session);

            _logger.LogInformation(
                "Session created for user {UserId}",
                user.Id);

            // Update Last Login
            user.LastLoginAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogError(
                    "Failed to update LastLoginAt for user {UserId}",
                    user.Id);

                return new LoginResponse
                {
                    Succeeded = false,
                    Message = "Unable to update login information."
                };
            }

            _logger.LogInformation(
                "User {Email} logged in successfully.",
                user.Email);

            return new LoginResponse
            {
                Succeeded = true,
                Message = "Login successful.",

                UserId = user.Id,
                Email = user.Email,

                AccessToken = accessToken,
                RefreshToken = refreshResult.RefreshToken,

                // Match this to your JwtSettings if you expose it later
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while logging in {Email}",
                request.Email);

            return new LoginResponse
            {
                Succeeded = false,
                Message = "An unexpected error occurred while processing your login request."
            };
        }
    }
}


//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using TaskManagement.Application.Common.Interfaces;
//using TaskManagement.Domain.Identity;

//namespace TaskManagement.Application.Features.Authentication.Login;

//public class LoginCommandHandler
//    : IRequestHandler<LoginCommand, LoginResponse>
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly IJwtService _jwtService;
//    private readonly IRefreshTokenService _refreshTokenService;
//    private readonly ISessionService _sessionService;
//    private readonly IClientInfoService _clientInfoService;

//    public LoginCommandHandler(
//        UserManager<ApplicationUser> userManager,
//        IJwtService jwtService,
//        IRefreshTokenService refreshTokenService,
//        ISessionService sessionService,
//        IClientInfoService clientInfoService)
//    {
//        _userManager = userManager;
//        _jwtService = jwtService;
//        _refreshTokenService = refreshTokenService;
//        _sessionService = sessionService;
//        _clientInfoService = clientInfoService;
//    }

//    public async Task<LoginResponse> Handle(
//        LoginCommand request,
//        CancellationToken cancellationToken)
//    {
//        // Find user
//        var user = await _userManager.FindByEmailAsync(request.Email);

//        if (user is null)
//        {
//            return new LoginResponse
//            {
//                Succeeded = false,
//                Message = "Invalid email or password."
//            };
//        }

//        // Check account status
//        if (!user.IsActive)
//        {
//            return new LoginResponse
//            {
//                Succeeded = false,
//                Message = "Your account is inactive."
//            };
//        }

//        // Check email confirmation
//        if (!await _userManager.IsEmailConfirmedAsync(user))
//        {
//            return new LoginResponse
//            {
//                Succeeded = false,
//                Message = "Please verify your email before logging in."
//            };
//        }

//        // Validate password
//        var passwordValid = await _userManager.CheckPasswordAsync(
//            user,
//            request.Password);

//        if (!passwordValid)
//        {
//            return new LoginResponse
//            {
//                Succeeded = false,
//                Message = "Invalid email or password."
//            };
//        }

//        // Get user roles
//        var roles = await _userManager.GetRolesAsync(user);

//        // Generate JWT
//        var accessToken = await _jwtService.GenerateAccessTokenAsync(
//            user.Id,
//            user.UserName!,
//            user.Email!,
//            roles);

//        // Get client information
//        var client = _clientInfoService.GetSessionInfo();

//        // Generate Refresh Token
//        var refreshResult = await _refreshTokenService.GenerateAsync(
//            user,
//            client.IpAddress ?? "Unknown");

//        // Create Session
//        var session = new UserSession
//        {
//            ApplicationUserId = user.Id,
//            RefreshTokenId = refreshResult.RefreshTokenEntity.Id,

//            DeviceName = client.DeviceName,
//            Browser = client.Browser,
//            OperatingSystem = client.OperatingSystem,
//            IpAddress = client.IpAddress,
//            UserAgent = client.UserAgent,

//            CreatedAt = DateTime.UtcNow,
//            LastActivityAt = DateTime.UtcNow,
//            ExpiresAt = refreshResult.RefreshTokenEntity.ExpiresAt,
//            IsActive = true
//        };

//        await _sessionService.CreateAsync(session);

//        // Update Last Login
//        user.LastLoginAt = DateTime.UtcNow;

//        var updateResult = await _userManager.UpdateAsync(user);

//        if (!updateResult.Succeeded)
//        {
//            return new LoginResponse
//            {
//                Succeeded = false,
//                Message = "Unable to update login information."
//            };
//        }

//        return new LoginResponse
//        {
//            Succeeded = true,
//            Message = "Login successful.",

//            UserId = user.Id,
//            Email = user.Email,

//            AccessToken = accessToken,
//            RefreshToken = refreshResult.RefreshToken,

//            // Temporary until IJwtService returns expiry
//            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
//        };
//    }
//}