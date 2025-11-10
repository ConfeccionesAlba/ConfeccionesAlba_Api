using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public record ItemCreateRequest(
    [FromForm] string Name,
    [FromForm] string Description,
    [FromForm] int CategoryId,
    [FromForm] decimal PriceReference,
    [FromForm] bool IsVisible,
    [FromForm] IFormFile File);

public static class CreateItem
{
    public static async Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, IImageProcessor imageProcessor, [FromForm] ItemCreateRequest itemRequest)
    {
        var response = new ApiResponse();
        
        try
        {
            // Get file
            var file = itemRequest.File;
            
            var newFileName = $"{Guid.NewGuid()}.webp"; // TODO: Extract file extension from here
            
            var url = await imageProcessor.ProcessAsync(newFileName, file.ContentType, file.OpenReadStream());

            // Save to database
            var image = new Image { Name = newFileName, Url = url };

            var newItem = new Item
            {
                Name = itemRequest.Name,
                Description = itemRequest.Description,
                CategoryId = itemRequest.CategoryId,
                PriceReference = itemRequest.PriceReference,
                IsVisible = itemRequest.IsVisible,
                Image = image,
            };

            db.Items.Add(newItem);
            await db.SaveChangesAsync();

            response.Result = newItem;
            response.StatusCode = HttpStatusCode.Created;

            return TypedResults.CreatedAtRoute(response, ItemsEndpointNames.GetItemById, new { newItem.Id });
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