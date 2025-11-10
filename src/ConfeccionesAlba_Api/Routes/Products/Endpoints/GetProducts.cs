using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public static class GetProducts
{
    public static async Task<Results<Ok<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db)
    {
        var response = new ApiResponse();

        try
        {
            var products = await db.Products.AsNoTracking().ToListAsync();
        
            response.StatusCode = HttpStatusCode.OK;
            response.Result = products;
        
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