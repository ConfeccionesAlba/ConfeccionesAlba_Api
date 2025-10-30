using System.Net;
using System.Security.Claims;
using System.Text;
using ConfeccionesAlba_Api.Configurations;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace ConfeccionesAlba_Api.Routes.Auth.Endpoints;

public static class LoginUser
{
    public static async
        Task<Results<Ok<ApiResponse>, BadRequest<ApiResponse>>> Handle(UserManager<ApplicationUser> userManager,
            IConfiguration configuration, LoginRequestDto model)
    {
        var response = new ApiResponse();
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() ??
                          throw new InvalidOperationException("Missing configuration settings");

        var userFromDb = await userManager.FindByEmailAsync(model.Email);
        if (userFromDb != null)
        {
            var isValid = await userManager.CheckPasswordAsync(userFromDb, model.Password);
            if (!isValid)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid Credentials");
                return TypedResults.BadRequest(response);
            }

            var roles = await userManager.GetRolesAsync(userFromDb);
           
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Sub, userFromDb.Id),
                new(JwtRegisteredClaimNames.Email, userFromDb.Email!),
                new(JwtRegisteredClaimNames.Name, userFromDb.Name),
                ..roles.Select(r => new Claim(ClaimTypes.Role, r)),
            ];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = credentials,
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
            };

            var tokenHandler = new JsonWebTokenHandler();
            
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);

            var loginResponse = new LoginResponseDto { Token = accessToken };

            response.Result = loginResponse;
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            return TypedResults.Ok(response);
        }

        response.StatusCode = HttpStatusCode.BadRequest;
        response.IsSuccess = false;
        response.ErrorMessages.Add("Invalid Credentials");
        return TypedResults.BadRequest(response);
    }
}