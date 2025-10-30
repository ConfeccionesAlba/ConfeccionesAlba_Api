using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace ConfeccionesAlba_Api.Routes.Auth.Endpoints;

public static class RegisterUser
{
    public static async Task<Results<Ok<ApiResponse>, BadRequest<ApiResponse>>> Handle(
        UserManager<ApplicationUser> userManager, RegisterRequestDto model)
    {
        var response = new ApiResponse();

        ApplicationUser newUser = new()
        {
            Email = model.Email,
            UserName = model.Email,
            Name = model.Name,
            NormalizedEmail = model.Email.ToUpper()
        };

        var result = await userManager.CreateAsync(newUser, model.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newUser, UserRoles.Publisher);

            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            return TypedResults.Ok(response);
        }

        foreach (var error in result.Errors)
        {
            response.ErrorMessages.Add(error.Description);
        }

        response.StatusCode = HttpStatusCode.BadRequest;
        response.IsSuccess = false;
        return TypedResults.BadRequest(response);
    }
}