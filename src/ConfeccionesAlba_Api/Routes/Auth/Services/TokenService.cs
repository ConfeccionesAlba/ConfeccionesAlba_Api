using System.Security.Claims;
using System.Text;
using ConfeccionesAlba_Api.Authorization;
using ConfeccionesAlba_Api.Configurations;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace ConfeccionesAlba_Api.Routes.Auth.Services;

public class TokenService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration)
{
    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() ??
                          throw new InvalidOperationException("Missing configuration settings");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);
        var permissions = new List<string>();

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            var claims = await roleManager.GetClaimsAsync(role);
            permissions.AddRange(claims.Where(c => c.Type == CustomClaimTypes.Permission).Select(c => c.Value));
        }

        var distinctPermissions = permissions.Distinct();

        List<Claim> claimsList =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.Name),
            ..roles.Select(r => new Claim(ClaimTypes.Role, r)),
            ..distinctPermissions.Select(p => new Claim(CustomClaimTypes.Permission, p))
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claimsList),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = credentials,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
        };

        var tokenHandler = new JsonWebTokenHandler();

        var accessToken = tokenHandler.CreateToken(tokenDescriptor);

        return accessToken;
    }
}