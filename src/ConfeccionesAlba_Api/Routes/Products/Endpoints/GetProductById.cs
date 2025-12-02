using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public static class GetProductById
{
    public static async Task<Results<Ok<ApiResponse<Product>>, NotFound<ApiResponse<Product>>, BadRequest<ApiResponse<Product>>, InternalServerError<ApiResponse<Product>>>> Handle(ApplicationDbContext db, int id)
    {

        if (id < 1)
        {
            return TypedResults.BadRequest(
                ApiResponse.Fail<Product>("Invalid Product Id"));
        }
        
        try
        {
            var product = await db.Products.FindAsync(id);
            
            if (product == null)
            {
                return TypedResults.NotFound(
                    ApiResponse.Fail<Product>("Product not found", HttpStatusCode.NotFound));
            }

            return TypedResults.Ok(
                ApiResponse.Success(product));
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<Product>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}