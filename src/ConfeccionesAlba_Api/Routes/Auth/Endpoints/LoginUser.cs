using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Auth;
using ConfeccionesAlba_Api.Routes.Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace ConfeccionesAlba_Api.Routes.Auth.Endpoints;

public static class LoginUser
{
    public static async
        Task<Results<Ok<ApiResponse>, BadRequest<ApiResponse>>> Handle(UserManager<ApplicationUser> userManager,
            TokenService tokenService, LoginRequestDto model)
    {
        var response = new ApiResponse();
        
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

            var accessToken = await tokenService.GenerateTokenAsync(userFromDb);

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