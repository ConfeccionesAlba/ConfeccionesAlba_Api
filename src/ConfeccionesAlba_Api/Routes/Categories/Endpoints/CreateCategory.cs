using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public static class CreateCategory
{
    public static async Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, CategoryCreateRequest categoryRequest)
    {
        var response = new ApiResponse();
        
        try
        {
            var category = new Category
            {
                Name = categoryRequest.Name,
                Description = categoryRequest.Description,
            };

            db.Categories.Add(category);
            await db.SaveChangesAsync();

            response.Result = category;
            response.StatusCode = HttpStatusCode.Created;

            return TypedResults.CreatedAtRoute(response, CategoriesEndpointNames.GetCategoryById, new { category.Id });
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