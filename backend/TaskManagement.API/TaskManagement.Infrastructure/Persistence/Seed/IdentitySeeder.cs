using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Persistence.Seed;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();

        var userManager = serviceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = serviceProvider
            .GetRequiredService<IConfiguration>();

        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager, configuration);
    }


    private static async Task SeedRolesAsync(
        RoleManager<ApplicationRole> roleManager)
    {
        string[] roles = 
        { 
            "Admin", 
            "User" 
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new ApplicationRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
            }
        }
    }


    private static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email = configuration["AdminUser:Email"];
        var password = configuration["AdminUser:Password"];


        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }


        var existingAdmin = await userManager
            .FindByEmailAsync(email);


        if (existingAdmin != null)
            return;


        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "System",
            LastName = "Administrator",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };


        var result = await userManager
            .CreateAsync(admin, password);


        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                result.Errors.Select(x => x.Description)));
        }


        await userManager.AddToRoleAsync(
            admin,
            "Admin");
    }
}