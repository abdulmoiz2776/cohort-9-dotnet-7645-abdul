using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;
using TaskManagement.Infrastructure.Authentication.Configurations;
using TaskManagement.Infrastructure.Authentication.Services;
using TaskManagement.Infrastructure.Authentication.Settings;
using TaskManagement.Infrastructure.Email.Models;
using TaskManagement.Infrastructure.Email.Services;
using TaskManagement.Infrastructure.Persistence.Contexts;
using TaskManagement.Infrastructure.Services;



namespace TaskManagement.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.Configure<JwtSettings>(
       configuration.GetSection(JwtSettings.SectionName));

        services.Configure<EmailSettings>(
    configuration.GetSection(EmailSettings.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.Configure<AppSettings>(
    configuration.GetSection(AppSettings.SectionName));


        services
      .AddIdentityCore<ApplicationUser>(options =>
      {
          // Password
          options.Password.RequiredLength = 8;
          options.Password.RequireUppercase = true;
          options.Password.RequireLowercase = true;
          options.Password.RequireDigit = true;
          options.Password.RequireNonAlphanumeric = true;

          // User
          options.User.RequireUniqueEmail = true;
          options.User.AllowedUserNameCharacters =
              "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

          // Lockout
          options.Lockout.MaxFailedAccessAttempts = 5;
          options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

          // SignIn
          options.SignIn.RequireConfirmedEmail = false;
      })
      .AddRoles<ApplicationRole>()
      .AddSignInManager<SignInManager<ApplicationUser>>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();


        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<EmailTemplateService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor();
        services.Configure<RefreshTokenSettings>(
    configuration.GetSection("RefreshTokenSettings"));

        services.AddScoped<ITokenGenerator, TokenGenerator>();

        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.AddScoped<ISessionService, SessionService>();

        services.AddScoped<IClientInfoService, ClientInfoService>();
        services.AddScoped<IApplicationUrlService, ApplicationUrlService>();

        return services;
    }
}