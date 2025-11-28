using ConfeccionesAlba_Api.Authorization;
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
            new IdentityRoleClaim<string> { Id = 1001, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.UsersRead.ToName() },
            new IdentityRoleClaim<string> { Id = 1002, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.UsersCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1003, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.UsersUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1004, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.UsersDelete.ToName() },

            new IdentityRoleClaim<string> { Id = 1005, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.CategoriesCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1006, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.CategoriesUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1007, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.CategoriesDelete.ToName() },

            new IdentityRoleClaim<string> { Id = 1008, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductsCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 1009, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductsUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 1010, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductsDelete.ToName() },
            new IdentityRoleClaim<string> { Id = 1011, RoleId = adminRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductImageUpload.ToName() },
            
            // Publisher Role
            new IdentityRoleClaim<string> { Id = 2001, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.CategoriesCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 2002, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.CategoriesUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 2003, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.CategoriesDelete.ToName() },

            new IdentityRoleClaim<string> { Id = 2004, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductsCreate.ToName() },
            new IdentityRoleClaim<string> { Id = 2005, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductsUpdate.ToName() },
            new IdentityRoleClaim<string> { Id = 2006, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductsDelete.ToName() },
            new IdentityRoleClaim<string> { Id = 2007, RoleId = publisherRoleId, ClaimType = CustomClaimTypes.Permission, ClaimValue = Permissions.ProductImageUpload.ToName() }
        );
    }
}