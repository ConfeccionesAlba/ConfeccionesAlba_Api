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
        var response = new ApiResponse<UploadImageResponse>();

        try
        {
            if (image is not { Length: > 0 })
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid image received");
                return TypedResults.BadRequest(response);
            }
            
            var productFromDb = await db.Products.FindAsync(id);
            
            if (productFromDb == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Product Id not found");
                return TypedResults.BadRequest(response);
            }

            var imageNameFromDb = productFromDb.Image.Name; // TODO: Extract file extension from here
            await imageProcessor.RemoveAsync(imageNameFromDb);

            var newFileName = $"{Guid.NewGuid().ToString()}.webp";
            var url = await imageProcessor.ProcessAsync(newFileName, image.ContentType, image.OpenReadStream());

            productFromDb.Image.Name = newFileName;
            productFromDb.Image.Url = url;
            
            await db.SaveChangesAsync();

            response.Result = new UploadImageResponse { Url = url };
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