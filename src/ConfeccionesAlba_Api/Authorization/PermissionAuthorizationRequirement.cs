using Microsoft.AspNetCore.Authorization;

namespace ConfeccionesAlba_Api.Authorization;

public class PermissionAuthorizationRequirement(params string[] allowedPermissions)
    : AuthorizationHandler<PermissionAuthorizationRequirement>, IAuthorizationRequirement
{
    public string[] AllowedPermissions { get; } = allowedPermissions;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        if (requirement.AllowedPermissions.Any(permission => PermissionIsFound(context, permission)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool PermissionIsFound(AuthorizationHandlerContext context, string permission)
    {
        var found = context.User.FindFirst(claim =>
            claim.Type == CustomClaimTypes.Permission &&
            claim.Value == permission) is not null;
        return found;
    }
}