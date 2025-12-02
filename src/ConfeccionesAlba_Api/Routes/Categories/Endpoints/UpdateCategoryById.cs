using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public record CategoryUpdateRequest(int Id, string Description);

public static class UpdateCategoryById
{
    public static async Task<Results<Ok<ApiResponse<Category>>, NotFound<ApiResponse<Category>>, BadRequest<ApiResponse<Category>>, InternalServerError<ApiResponse<Category>>>> Handle(ApplicationDbContext db, CategoryUpdateRequest categoryRequest, int id)
    {
        if (categoryRequest.Id != id)
        {
            return TypedResults.BadRequest(
                ApiResponse.Fail<Category>("Invalid category id"));
        }
        
        try
        {
            var categoryFromDb = await db.Categories.FindAsync(id);
            
            if (categoryFromDb == null)
            {
                return TypedResults.NotFound(
                    ApiResponse.Fail<Category>("Category not found", HttpStatusCode.NotFound));
            }

            if (!string.IsNullOrEmpty(categoryRequest.Description))
            {
                categoryFromDb.Description = categoryRequest.Description;
            }

            await db.SaveChangesAsync();
            
            return TypedResults.Ok(
                ApiResponse.Success<Category>());
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<Category>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}