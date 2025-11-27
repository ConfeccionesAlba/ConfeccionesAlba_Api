using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public static class GetCategoryById
{
    public static async Task<Results<Ok<ApiResponse<Category>>, NotFound<ApiResponse<Category>>, BadRequest<ApiResponse<Category>>, InternalServerError<ApiResponse<Category>>>> Handle(ApplicationDbContext db, int id)
    {
        var response = new ApiResponse<Category>();

        if (id < 1)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessages.Add("Invalid category Id");
            return TypedResults.BadRequest(response);
        }
        
        try
        {
            var category = await db.Categories.FindAsync(id);
            
            if (category == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Category not found");
                return TypedResults.NotFound(response);
            }

            response.Result = category;
            response.StatusCode = HttpStatusCode.OK;
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