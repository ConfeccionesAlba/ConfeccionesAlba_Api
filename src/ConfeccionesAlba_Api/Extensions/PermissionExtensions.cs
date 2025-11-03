using ConfeccionesAlba_Api.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace ConfeccionesAlba_Api.Extensions;

public static class PermissionExtensions
{
    public static string ToName(this Permission p) => p switch
    {
        Permission.UsersRead => "users:read",
        Permission.UsersCreate => "users:create",
        Permission.UsersUpdate => "users:update",
        Permission.UsersDelete => "users:delete",
        Permission.CategoryCreate => "category:create",
        Permission.CategoryUpdate => "category:update",
        Permission.CategoryDelete => "category:delete",
        Permission.ItemCreate => "item:create",
        Permission.ItemUpdate => "item:update",
        Permission.ItemDelete => "item:delete",
        _ => throw new ArgumentOutOfRangeException(nameof(p))
    };
    
    public static void RequirePermission(this AuthorizationPolicyBuilder builder, params string[] allowedPermissions)
    {
        builder.AddRequirements(new PermissionAuthorizationRequirement(allowedPermissions));
    }
    
    public static void RequirePermission(this AuthorizationPolicyBuilder builder, params Permission[] allowedPermissions)
    {
        var permissions = allowedPermissions.Select(p => p.ToName()).ToArray();
        builder.AddRequirements(new PermissionAuthorizationRequirement(permissions));
    }
}