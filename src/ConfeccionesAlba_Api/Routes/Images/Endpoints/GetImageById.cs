using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Images.Endpoints;

public static class GetImageById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(
        ApplicationDbContext db,
        int id)
    {
        var response = new ApiResponse();

        if (id < 1)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessages.Add("Invalid image Id");
            return TypedResults.BadRequest(response);
        }
        
        try
        {
            var image = await db.Images.FindAsync(id);
            
            if (image == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("image not found");
                return TypedResults.NotFound(response);
            }

            response.Result = image;
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