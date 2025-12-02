using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace ConfeccionesAlba_Api.Routes.Auth.Endpoints;

public record RegisterRequest(string Name, string Email, string Password);

public static class RegisterUser
{
    public static async Task<Results<Ok<ApiResponse<object>>, BadRequest<ApiResponse<object>>, InternalServerError<ApiResponse<object>>>> Handle(
        UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, RegisterRequest model)
    {

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
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
                await userManager.AddToRoleAsync(newUser, UserRolesValues.Publisher);
                await transaction.CommitAsync();

                return TypedResults.Ok(
                    ApiResponse.Success<object>());
            }

            await transaction.RollbackAsync();
            return TypedResults.BadRequest(
                ApiResponse.Fail<object>(result.Errors.Select(e => e.Description)));
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            
            return TypedResults.BadRequest(
                ApiResponse.Fail<object>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}