using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace ConfeccionesAlba_Api.Routes.Auth.Endpoints;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token);

public static class LoginUser
{
    public static async
        Task<Results<Ok<ApiResponse<LoginResponse>>, BadRequest<ApiResponse<LoginResponse>>>> Handle(UserManager<ApplicationUser> userManager,
            TokenService tokenService, LoginRequest model)
    {
        var userFromDb = await userManager.FindByEmailAsync(model.Email);
        if (userFromDb == null)
        {
            return TypedResults.BadRequest(
                ApiResponse.Fail<LoginResponse>("Invalid Credentials"));
        }
        
        var isValid = await userManager.CheckPasswordAsync(userFromDb, model.Password);
        if (!isValid)
        {
            return TypedResults.BadRequest(
                ApiResponse.Fail<LoginResponse>("Invalid Credentials"));
        }

        var accessToken = await tokenService.GenerateTokenAsync(userFromDb);

        var loginResponse = new LoginResponse(accessToken);
            
        return TypedResults.Ok(
            ApiResponse.Success(loginResponse));
    }
}