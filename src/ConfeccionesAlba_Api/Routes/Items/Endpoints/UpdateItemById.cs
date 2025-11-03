using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public record ItemUpdateRequest(int Id, string Description, int CategoryId, decimal PriceReference, bool IsVisible);

public static class UpdateItemById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, ItemUpdateRequest itemRequest, int id)
    {
        var response = new ApiResponse();

        if (itemRequest.Id != id)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessages.Add("Invalid item id");

            return TypedResults.BadRequest(response);
        }
        
        try
        {
            var itemFromDb = await db.Items.FindAsync(id);
            
            if (itemFromDb == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Item not found");
                return TypedResults.NotFound(response);
            }

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

            await db.SaveChangesAsync();
            
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