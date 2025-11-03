using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public record CategoryUpdateRequest(int Id, string Description);

public static class UpdateCategoryById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, CategoryUpdateRequest categoryRequest, int id)
    {
        var response = new ApiResponse();

        if (categoryRequest.Id != id)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessages.Add("Invalid category id");

            return TypedResults.BadRequest(response);
        }
        
        try
        {
            var categoryFromDb = await db.Categories.FindAsync(id);
            
            if (categoryFromDb == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Category not found");
                return TypedResults.NotFound(response);
            }

            if (!string.IsNullOrEmpty(categoryRequest.Description))
            {
                categoryFromDb.Description = categoryRequest.Description;
            }

            await db.SaveChangesAsync();
            
            response.StatusCode = HttpStatusCode.NoContent;

            return TypedResults.Ok(response);
        }
        catch (Exception exception)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages = [exception.Message];
            return TypedResults.InternalServerError(response);
        }
    }
}