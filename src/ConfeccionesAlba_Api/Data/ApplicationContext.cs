using ConfeccionesAlba_Api.Common;
using ConfeccionesAlba_Api.Data.Interceptors;
using ConfeccionesAlba_Api.Extensions;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Data;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.AddInterceptors(new AuditableEntityInterceptor());

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Seed roles
        var adminRole = new IdentityRole(UserRolesValues.Admin) { Id = "f4dbaa01-dcae-40a5-844e-d8a719393af4", NormalizedName = UserRolesValues.Admin.ToUpperInvariant() };
        var publisherRole = new IdentityRole(UserRolesValues.Publisher) { Id = "1dacfc02-50b8-43da-a050-b0fa556224b7", NormalizedName = UserRolesValues.Publisher.ToUpperInvariant() };

        builder.Entity<IdentityRole>().HasData(adminRole, publisherRole);

        // Seed permissions via RoleClaims
        builder.Entity<IdentityRoleClaim<string>>().HasData(
            // Admin Role
            new IdentityRoleClaim<string> { Id = 1001, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersRead.ToName() },
            new IdentityRoleClaim<string> { Id = 1002, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1003, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1004, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersDelete.ToName() },
            
            new IdentityRoleClaim<string> { Id = 1005, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1006, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1007, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryDelete.ToName() },
            
            new IdentityRoleClaim<string> { Id = 1008, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1009, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1010, RoleId = adminRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemDelete.ToName() },
            
            // Publisher Role
            new IdentityRoleClaim<string> { Id = 2001, RoleId = publisherRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 2002, RoleId = publisherRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 2003, RoleId = publisherRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryDelete.ToName() },
            
            new IdentityRoleClaim<string> { Id = 2004, RoleId = publisherRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 2005, RoleId = publisherRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 2006, RoleId = publisherRole.Id, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemDelete.ToName() }
        );
    }
}