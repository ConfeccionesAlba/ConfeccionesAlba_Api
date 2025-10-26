using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Categories;
using ConfeccionesAlba_Api.Models.Dtos.Categories.Validators;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Categories.Endpoints;

public static class CreateCategory
{
    public static async Task<Results<CreatedAtRoute<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, CategoryCreateDtoValidator validator, CategoryCreateDto categoryDto)
    {
        var response = new ApiResponse();

        var validationResult = await validator.ValidateAsync(categoryDto).ConfigureAwait(false);
        
        if (!validationResult.IsValid)
        {
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessages.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

            return TypedResults.BadRequest(response);
        }
        
        try
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
            };

            db.Categories.Add(category);
            await db.SaveChangesAsync();

            response.Result = category;
            response.StatusCode = HttpStatusCode.Created;

            return TypedResults.CreatedAtRoute(response, CategoriesEndpointNames.GetCategoryById, new { category.Id });
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