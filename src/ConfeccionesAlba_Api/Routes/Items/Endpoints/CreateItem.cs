using System.Net;
using System.Text.Json;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public record ItemCreateRequest(string Name, string Description, int CategoryId, decimal PriceReference, bool IsVisible);

public static class CreateItem
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };
    
    public static async Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(HttpRequest request, ApplicationDbContext db, IImageProcessor imageProcessor)
    {
        var response = new ApiResponse();
        
        try
        {
            var form = await request.ReadFormAsync();
            
            // Get product JSON field
            var itemJson = form["item"].ToString();
            var itemRequest = JsonSerializer.Deserialize<ItemCreateRequest>(itemJson, SerializerOptions);

            if (itemRequest is null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid item data.");

                return TypedResults.BadRequest(response);
            }
            
            // Get file
            var file = form.Files.GetFile("image");
            
            if (file is null || file.Length == 0)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("No image file provided");
                return TypedResults.BadRequest(response);
            }
            
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