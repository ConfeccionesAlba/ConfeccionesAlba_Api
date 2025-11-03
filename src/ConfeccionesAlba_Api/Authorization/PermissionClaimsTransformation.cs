using System.Security.Claims;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace ConfeccionesAlba_Api.Authorization;

public class PermissionClaimsTransformation(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
    : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity!;
        var user = await userManager.GetUserAsync(principal);
        if (user == null)
        {
            return principal;
        }

        var roles = await userManager.GetRolesAsync(user);
        var permissions = new HashSet<string>();

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            var claims = await roleManager.GetClaimsAsync(role);
            foreach (var c in claims.Where(c => c.Type == CustomClaimTypes.Permission))
            {
                permissions.Add(c.Value);
            }
        }

        foreach (var p in permissions)
        {
            if (!identity.HasClaim(CustomClaimTypes.Permission, p))
            {
                identity.AddClaim(new Claim(CustomClaimTypes.Permission, p));
            }
        }

        return principal;
    }
}