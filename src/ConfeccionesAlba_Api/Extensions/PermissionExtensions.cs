using ConfeccionesAlba_Api.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace ConfeccionesAlba_Api.Extensions;

public static class PermissionExtensions
{
    public static string ToName(this Permissions p) => p switch
    {
        Permissions.UsersRead => "users:read",
        Permissions.UsersCreate => "users:create",
        Permissions.UsersUpdate => "users:update",
        Permissions.UsersDelete => "users:delete",
        Permissions.CategoriesCreate => "categories:create",
        Permissions.CategoriesUpdate => "categories:update",
        Permissions.CategoriesDelete => "categories:delete",
        Permissions.ProductsCreate => "products:create",
        Permissions.ProductsUpdate => "products:update",
        Permissions.ProductsDelete => "products:delete",
        _ => throw new ArgumentOutOfRangeException(nameof(p))
    };
    
    public static void RequirePermission(this AuthorizationPolicyBuilder builder, params string[] allowedPermissions)
    {
        builder.AddRequirements(new PermissionAuthorizationRequirement(allowedPermissions));
    }
    
    public static void RequirePermission(this AuthorizationPolicyBuilder builder, params Permissions[] allowedPermissions)
    {
        var permissions = allowedPermissions.Select(p => p.ToName()).ToArray();
        builder.AddRequirements(new PermissionAuthorizationRequirement(permissions));
    }
}