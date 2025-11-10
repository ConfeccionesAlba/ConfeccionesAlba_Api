using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public class ProductUpdateRequest
{
    [FromForm] public int Id { get; set; }
    [FromForm] public string? Description { get; set; }
    [FromForm] public int? CategoryId { get; set; }
    [FromForm] public decimal? PriceReference { get; set; }
    [FromForm] public bool? IsVisible { get; set; }
    [FromForm] public IFormFile? File { get; set; }
}

public static class UpdateProductById
{
    public static async Task<Results<Ok<ApiResponse>, NotFound<ApiResponse>, BadRequest<ApiResponse>, InternalServerError<ApiResponse>>> Handle(ApplicationDbContext db, IImageProcessor imageProcessor, [FromForm] ProductUpdateRequest productRequest, int id)
    {
        var response = new ApiResponse();
        
        try
        {
            if (productRequest.Id != id)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid Product id");

                return TypedResults.BadRequest(response);
            }
            
            var productFromDb = await db.Products.FindAsync(id);
            
            if (productFromDb == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Product not found");
                return TypedResults.NotFound(response);
            }

            // Update item image
            var file = productRequest.File;
            if (file is { Length: > 0 })
            {
                var imageNameFromDb = productFromDb.Image.Name; // TODO: Extract file extension from here
                await imageProcessor.RemoveAsync(imageNameFromDb);
                
                var newFileName = $"{Guid.NewGuid().ToString()}.webp";
                var url = await imageProcessor.ProcessAsync(newFileName, file.ContentType, file.OpenReadStream());
                
                productFromDb.Image.Name = newFileName;
                productFromDb.Image.Url = url;
            }

            // Update item properties
            if (!string.IsNullOrEmpty(productRequest.Description))
            {
                productFromDb.Description = productRequest.Description;
            }

            if (productRequest.CategoryId.HasValue)
            {
                productFromDb.CategoryId = productRequest.CategoryId.Value;
            }

            if (productRequest.PriceReference.HasValue)
            {
                productFromDb.PriceReference = productRequest.PriceReference.Value;
            }

            if (productRequest.IsVisible.HasValue)
            {
                productFromDb.IsVisible = productRequest.IsVisible.Value;
            }

            // Save to database
            var entry = db.Entry(productFromDb);

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