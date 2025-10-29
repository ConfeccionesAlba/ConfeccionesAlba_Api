using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ConfeccionesAlba_Api.Data;
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
        var secretKey = configuration.GetValue<string>("ApiSettings:Secret") ??
                        throw new InvalidOperationException("ApiSettings:Secret");

        var userFromDb = await userManager.FindByEmailAsync(model.Email);
        if (userFromDb != null)
        {
            var isValid = await userManager.CheckPasswordAsync(userFromDb, model.Password);
            if (!isValid)
            {
                response.Result = new LoginResponseDto();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid Credentials");
                return TypedResults.BadRequest(response);
            }

            JwtSecurityTokenHandler tokenHandler = new();
            var key = Encoding.ASCII.GetBytes(secretKey);

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
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
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

        response.Result = new LoginResponseDto();
        response.StatusCode = HttpStatusCode.BadRequest;
        response.IsSuccess = false;
        response.ErrorMessages.Add("Invalid Credentials");
        return TypedResults.BadRequest(response);
    }
}