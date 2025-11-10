using System.Net;
using System.Text.Json;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using ConfeccionesAlba_Api.Services.S3.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public class ItemUpdateRequest
{
    [FromForm] public int Id { get; set; }
    [FromForm] public string? Description { get; set; }
    [FromForm] public int? CategoryId { get; set; }
    [FromForm] public decimal? PriceReference { get; set; }
    [FromForm] public bool? IsVisible { get; set; }
    [FromForm] public IFormFile? File { get; set; }
}

public static class UpdateItemById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, IImageProcessor imageProcessor, IS3Client s3Client, [FromForm] ItemUpdateRequest itemRequest, int id)
    {
        var response = new ApiResponse();
        
        try
        {
            if (itemRequest.Id != id)
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

            // Update item image
            var file = itemRequest.File;
            if (file is { Length: > 0 })
            {
                var imageNameFromDb = itemFromDb.Image.Name; // TODO: Extract file extension from here
                await s3Client.RemoveImage(imageNameFromDb);
                
                var newFileName = $"{Guid.NewGuid().ToString()}.webp";
                var url = await imageProcessor.ProcessAsync(newFileName, file.ContentType, file.OpenReadStream());
                
                itemFromDb.Image.Name = newFileName;
                itemFromDb.Image.Url = url;
            }

            // Update item properties
            if (!string.IsNullOrEmpty(itemRequest.Description))
            {
                itemFromDb.Description = itemRequest.Description;
            }

            if (itemRequest.CategoryId.HasValue)
            {
                itemFromDb.CategoryId = itemRequest.CategoryId.Value;
            }

            if (itemRequest.PriceReference.HasValue)
            {
                itemFromDb.PriceReference = itemRequest.PriceReference.Value;
            }

            if (itemRequest.IsVisible.HasValue)
            {
                itemFromDb.IsVisible = itemRequest.IsVisible.Value;
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