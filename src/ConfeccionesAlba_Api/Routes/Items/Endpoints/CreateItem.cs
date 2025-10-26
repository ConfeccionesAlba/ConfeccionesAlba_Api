using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Items;
using ConfeccionesAlba_Api.Models.Dtos.Items.Validators;
using ConfeccionesAlba_Api.Routes.Categories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Items.Endpoints;

public static class CreateItem
{
    public static async Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, ItemCreateDtoValidator validator, ItemCreateDto itemDto)
    {
        var response = new ApiResponse();

        var validationResult = await validator.ValidateAsync(itemDto).ConfigureAwait(false);
        
        if (!validationResult.IsValid)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessages.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

            return TypedResults.BadRequest(response);
        }
        
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

            return TypedResults.CreatedAtRoute(response, CategoriesEndpointNames.GetCategoryById, new { newItem.Id });
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