using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public class UploadImageResponse
{
    public string Url { get; set; } = string.Empty;
}

public static class UploadProductImage
{
    public static async
        Task<Results<
            Ok<ApiResponse<UploadImageResponse>>,
            NotFound<ApiResponse<UploadImageResponse>>,
            BadRequest<ApiResponse<UploadImageResponse>>,
            InternalServerError<ApiResponse<UploadImageResponse>>>>
        Handle(ApplicationDbContext db,
            IImageProcessor imageProcessor,
            [FromRoute] int id,
            [FromForm] IFormFile image)
    {
        try
        {
            if (image is not { Length: > 0 })
            {
                return TypedResults.BadRequest(
                    ApiResponse.Fail<UploadImageResponse>("Invalid image received"));
            }
            
            var productFromDb = await db.Products.FindAsync(id);
            
            if (productFromDb == null)
            {
                return TypedResults.BadRequest(
                    ApiResponse.Fail<UploadImageResponse>("Product Id not found"));
            }

            var imageNameFromDb = productFromDb.Image.Name; // TODO: Extract file extension from here
            await imageProcessor.RemoveAsync(imageNameFromDb);

            var newFileName = $"{Guid.NewGuid().ToString()}.webp";
            var url = await imageProcessor.ProcessAsync(newFileName, image.ContentType, image.OpenReadStream());

            productFromDb.Image.Name = newFileName;
            productFromDb.Image.Url = url;
            
            await db.SaveChangesAsync();

            return TypedResults.Ok(
                ApiResponse.Success(new UploadImageResponse { Url = url }));
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<UploadImageResponse>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}