using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Images.Endpoints;

public static class CreateImage
{
    public static async
        Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(
            HttpRequest request,
            ApplicationDbContext db,
            IImageProcessor imageProcessor)
    {
        var response = new ApiResponse();
        
        try
        {
            var form = await request.ReadFormAsync();
            var file = form.Files.GetFile("file");
            var newFileName = $"{Guid.NewGuid()}.webp"; // New random name (.webp)

            if (file is null || file.Length == 0)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("No image file provided");
                return TypedResults.BadRequest(response);
            }

            var url = await imageProcessor.ProcessAsync(newFileName, file.ContentType, file.OpenReadStream());
            
            var image = new Image
            {
                Name = newFileName,
                Url = url
            };

            db.Images.Add(image);
            await db.SaveChangesAsync();

            response.Result = image;
            response.StatusCode = HttpStatusCode.Created;

            return TypedResults.CreatedAtRoute(response, ImagesEndpointNames.GetImageById, new { image.Id });
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