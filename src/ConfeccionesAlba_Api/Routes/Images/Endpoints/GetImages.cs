using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Images.Endpoints;

public static class GetImages
{
    public static async Task<Results<Ok<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db)
    {
        var response = new ApiResponse();

        try
        {
            var images = await db.Images.AsNoTracking().ToListAsync();
        
            response.StatusCode = HttpStatusCode.OK;
            response.Result = images;
        
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