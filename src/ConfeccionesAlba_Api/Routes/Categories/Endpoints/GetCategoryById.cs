using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public static class GetCategoryById
{
    public static async Task<Results<Ok<ApiResponse<Category>>, NotFound<ApiResponse<Category>>, BadRequest<ApiResponse<Category>>, InternalServerError<ApiResponse<Category>>>> Handle(ApplicationDbContext db, int id)
    {
        if (id < 1)
        {
            return TypedResults.BadRequest(
                ApiResponse.Fail<Category>("Invalid category Id"));
        }
        
        try
        {
            var category = await db.Categories.FindAsync(id);
            
            if (category == null)
            {
                return TypedResults.NotFound(
                    ApiResponse.Fail<Category>("Category not found", HttpStatusCode.NotFound));
            }

            return TypedResults.Ok(ApiResponse.Success(category));
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<Category>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}