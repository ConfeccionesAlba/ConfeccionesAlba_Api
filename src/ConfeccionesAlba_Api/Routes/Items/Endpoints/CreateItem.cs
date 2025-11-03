using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public record ItemCreateRequest(string Name, string Description, int CategoryId, decimal PriceReference, bool IsVisible);

public static class CreateItem
{
    public static async Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, ItemCreateRequest itemRequest)
    {
        var response = new ApiResponse();
        
        try
        {
            var newItem = new Item
            {
                Name = itemRequest.Name,
                Description = itemRequest.Description,
                CategoryId = itemRequest.CategoryId,
                PriceReference = itemRequest.PriceReference,
                IsVisible = itemRequest.IsVisible,
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