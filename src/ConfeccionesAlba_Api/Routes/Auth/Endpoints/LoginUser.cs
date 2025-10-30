using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ConfeccionesAlba_Api.Configurations;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

            JwtSecurityTokenHandler tokenHandler = new();

            var roles = await userManager.GetRolesAsync(userFromDb);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("fullname", userFromDb.Name),
                    new Claim("id", userFromDb.Id),
                    new Claim(ClaimTypes.Email, userFromDb.Email!),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = configuration.GetValue<string>(jwtSettings.Issuer),
                Audience = configuration.GetValue<string>(jwtSettings.Audience),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDto loginResponse = new()
            {
                Email = userFromDb.Email!,
                Token = tokenHandler.WriteToken(token),
                Role = userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault()!
            };

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