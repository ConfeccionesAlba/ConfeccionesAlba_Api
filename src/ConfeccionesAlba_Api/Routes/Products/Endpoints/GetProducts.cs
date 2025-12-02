using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public static class GetProducts
{
    public static async Task<Results<Ok<ApiResponse<Product[]>>, InternalServerError<ApiResponse<Product[]>>>> Handle(ApplicationDbContext db)
    {
        try
        {
            var products = await db.Products.AsNoTracking().ToArrayAsync();
        
            return TypedResults.Ok(
                ApiResponse.Success(products));
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<Product[]>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}