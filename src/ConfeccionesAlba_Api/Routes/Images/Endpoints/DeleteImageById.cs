using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.S3.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Images.Endpoints;

public static class DeleteImageById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(
        ApplicationDbContext db, 
        IS3Client s3Client,
        int id)
    {
        var response = new ApiResponse();
        
        try
        {
            var image = await db.Images.FindAsync(id);
            
            if (image == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Image not found");
                return TypedResults.NotFound(response);
            }

            await s3Client.RemoveImage(image.Name);

            db.Remove(image);
            await db.SaveChangesAsync();
            
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