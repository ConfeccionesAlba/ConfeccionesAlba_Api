using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task EnsureNoPendingMigrationsOrFail(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var migrations = await db.Database.GetPendingMigrationsAsync();
        if (migrations.Any())
        {
            throw new InvalidOperationException("Migrations are not supported.");
        }
    }

    public static async Task SeedRoles(this IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new[] { UserRolesValues.Admin, UserRolesValues.Publisher, UserRolesValues.Customer };

        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task EnsureAdminUserAndRole(this IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        var adminEmail = configuration.GetValue<string>("InitialAdmin:Email");

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var adminName = configuration.GetValue<string>("InitialAdmin:Name");
            var adminPassword = configuration.GetValue<string>("InitialAdmin:Password");

            adminUser = new ApplicationUser
            {
                Name = adminName,
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, UserRolesValues.Admin);
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                logger.LogError("Failed adding initial admin user `{User}`. Reasons: {Errors}", adminUser, errors);
                
                throw new InvalidOperationException($"Failed to create admin user: {adminEmail}");
            }
        }
    }
}