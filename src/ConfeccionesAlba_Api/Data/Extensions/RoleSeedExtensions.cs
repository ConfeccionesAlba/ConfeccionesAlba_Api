using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Data.Extensions;

public static class RoleSeedExtensions
{
    public static void SeedRoles(this ModelBuilder builder)
    {
        var adminRole = new IdentityRole(UserRolesValues.Admin)
        {
            Id = DefaultRolesId.AdminRoleId,
            NormalizedName = UserRolesValues.Admin.ToUpperInvariant()
        };

        var publisherRole = new IdentityRole(UserRolesValues.Publisher)
        {
            Id = DefaultRolesId.PublisherRoleId,
            NormalizedName = UserRolesValues.Publisher.ToUpperInvariant()
        };

        builder.Entity<IdentityRole>().HasData(adminRole, publisherRole);
    }
}