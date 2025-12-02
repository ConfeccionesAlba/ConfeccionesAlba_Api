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
        try
        {
            var categories = await db.Categories.AsNoTracking().ToArrayAsync();

            return TypedResults.Ok(
                ApiResponse.Success(categories));
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<Category[]>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}