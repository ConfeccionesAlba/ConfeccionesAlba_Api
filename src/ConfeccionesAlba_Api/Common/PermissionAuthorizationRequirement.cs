using Microsoft.AspNetCore.Authorization;

namespace ConfeccionesAlba_Api.Common;

public class PermissionAuthorizationRequirement(params string[] allowedPermissions)
    : AuthorizationHandler<PermissionAuthorizationRequirement>, IAuthorizationRequirement
{
    public string[] AllowedPermissions { get; } = allowedPermissions;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        foreach (var permission in requirement.AllowedPermissions)
        {
            var found = context.User.FindFirst(claim =>
                claim.Type == CustomClaimTypes.Permission &&
                claim.Value == permission) is not null;

            if (found)
            {
                context.Succeed(requirement);
                break;
            }
        }
        return Task.CompletedTask;
    }
}