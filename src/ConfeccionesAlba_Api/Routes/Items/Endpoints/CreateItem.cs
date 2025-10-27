using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Items;
using ConfeccionesAlba_Api.Routes.Categories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public static class CreateItem
{
    public static async Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, ItemCreateDto itemDto)
    {
        var response = new ApiResponse();
        
        try
        {
            var newItem = new Item
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                CategoryId = itemDto.CategoryId,
                PriceReference = itemDto.PriceReference,
                IsVisible = itemDto.IsVisible,
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