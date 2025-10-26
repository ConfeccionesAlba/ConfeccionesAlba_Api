using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Items;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public static class UpdateItemById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, ItemUpdateDto itemDto, int id)
    {
        var response = new ApiResponse();

        if (itemDto.Id != id)
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

            if (!string.IsNullOrEmpty(itemDto.Description))
            {
                itemFromDb.Description = itemDto.Description;
            }

            itemFromDb.CategoryId = itemDto.CategoryId;
            
            itemFromDb.PriceReference = itemDto.PriceReference;

            if (itemFromDb.IsVisible != itemDto.IsVisible)
            {
                itemFromDb.IsVisible = itemDto.IsVisible;
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