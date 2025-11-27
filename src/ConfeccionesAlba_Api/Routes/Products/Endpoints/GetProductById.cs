using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public static class GetProductById
{
    public static async Task<Results<Ok<ApiResponse<Product>>, NotFound<ApiResponse<Product>>, BadRequest<ApiResponse<Product>>, InternalServerError<ApiResponse<Product>>>> Handle(ApplicationDbContext db, int id)
    {
        var response = new ApiResponse<Product>();

        if (id < 1)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessages.Add("Invalid Product Id");
            return TypedResults.BadRequest(response);
        }
        
        try
        {
            var product = await db.Products.FindAsync(id);
            
            if (product == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Product not found");
                return TypedResults.NotFound(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            response.Result = product;
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