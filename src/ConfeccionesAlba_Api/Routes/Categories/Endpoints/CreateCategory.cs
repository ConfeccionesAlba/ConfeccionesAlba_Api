using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public static class CreateCategory
{
    public static async Task<Results<CreatedAtRoute<ApiResponse<Category>>, BadRequest<ApiResponse<Category>>, InternalServerError<ApiResponse<Category>>>> Handle(ApplicationDbContext db, CategoryCreateRequest categoryRequest)
    {
        try
        {
            var category = new Category
            {
                Name = categoryRequest.Name,
                Description = categoryRequest.Description,
            };

            db.Categories.Add(category);
            await db.SaveChangesAsync();

            return TypedResults.CreatedAtRoute(
                ApiResponse.Success(category, HttpStatusCode.Created),
                CategoriesEndpointNames.GetCategoryById,
                new { category.Id });
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<Category>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}