using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public static class GetCategories
{
    public static async Task<Results<Ok<ApiResponse<Category[]>>, InternalServerError<ApiResponse<Category[]>>>> Handle(ApplicationDbContext db)
    {
        var response = new ApiResponse<Category[]>();

        try
        {
            var categories = await db.Categories.AsNoTracking().ToArrayAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.Result = categories;
        
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