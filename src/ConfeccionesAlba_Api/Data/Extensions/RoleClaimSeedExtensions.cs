using ConfeccionesAlba_Api.Common;
using ConfeccionesAlba_Api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Data.Extensions;

public static class RoleClaimSeedExtensions
{
    public static void SeedRoleClaims(this ModelBuilder builder)
    {
        const string adminRoleId = DefaultRolesId.AdminRoleId;
        const string publisherRoleId = DefaultRolesId.PublisherRoleId;

        builder.Entity<IdentityRoleClaim<string>>().HasData(
            // Admin Role
            new IdentityRoleClaim<string> { Id = 1001, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersRead.ToName() },
            new IdentityRoleClaim<string> { Id = 1002, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1003, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1004, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.UsersDelete.ToName() },

            new IdentityRoleClaim<string> { Id = 1005, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1006, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1007, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryDelete.ToName() },

            new IdentityRoleClaim<string> { Id = 1008, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1009, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1010, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemDelete.ToName() },

            // Publisher Role
            new IdentityRoleClaim<string> { Id = 2001, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 2002, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 2003, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.CategoryDelete.ToName() },

            new IdentityRoleClaim<string> { Id = 2004, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 2005, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 2006, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permission.ItemDelete.ToName() }
        );
    }
}