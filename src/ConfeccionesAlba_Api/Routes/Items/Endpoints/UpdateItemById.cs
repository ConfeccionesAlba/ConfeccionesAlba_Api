using System.Net;
using System.Text.Json;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using ConfeccionesAlba_Api.Services.S3.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public record ItemUpdateRequest(int Id, string Description, int CategoryId, decimal PriceReference, bool IsVisible);

public static class UpdateItemById
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(HttpRequest request, ApplicationDbContext db, IImageProcessor imageProcessor, IS3Client s3Client, int id)
    {
        var response = new ApiResponse();
        
        try
        {
            var form = await request.ReadFormAsync();
            
            // Get product JSON field
            var itemJson = form["item"].ToString();
            var itemRequest = JsonSerializer.Deserialize<ItemUpdateRequest>(itemJson, SerializerOptions);
        
            if (itemRequest != null && itemRequest.Id != id)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid item id");

                return TypedResults.BadRequest(response);
            }
            
            var itemFromDb = await db.Items.FindAsync(id);
            
            if (itemFromDb == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Item not found");
                return TypedResults.NotFound(response);
            }
            
            // Get file
            var file = form.Files.GetFile("image");

            // Update item image
            if (file is { Length: > 0 })
            {
                var imageNameFromDb = $"{itemFromDb.Image.Name}.webp"; // TODO: Extract file extension from here
                await s3Client.RemoveImage(imageNameFromDb);
                
                var newFileName = Guid.NewGuid().ToString();
                var url = await imageProcessor.ProcessAsync(newFileName, file.ContentType, file.OpenReadStream());
                
                itemFromDb.Image.Name = newFileName;
                itemFromDb.Image.Url = url;
            }

            // Update item properties
            if (itemRequest != null)
            {
                if (!string.IsNullOrEmpty(itemRequest.Description))
                {
                    itemFromDb.Description = itemRequest.Description;
                }

                itemFromDb.CategoryId = itemRequest.CategoryId;
            
                itemFromDb.PriceReference = itemRequest.PriceReference;

                if (itemFromDb.IsVisible != itemRequest.IsVisible)
                {
                    itemFromDb.IsVisible = itemRequest.IsVisible;
                }
            }

            // Save to database
            var entry = db.Entry(itemFromDb);

            if (entry.State == EntityState.Modified)
            {
                await db.SaveChangesAsync();
            }
            else
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("No changes detected in submitted data");

                return TypedResults.BadRequest(response);
            }
            
            response.StatusCode = HttpStatusCode.NoContent;

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