using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Categories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public static class UpdateCategoryById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, CategoryUpdateDto categoryDto, int id)
    {
        var response = new ApiResponse();

        if (categoryDto.Id != id)
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

            if (!string.IsNullOrEmpty(categoryDto.Description))
            {
                categoryFromDb.Description = categoryDto.Description;
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