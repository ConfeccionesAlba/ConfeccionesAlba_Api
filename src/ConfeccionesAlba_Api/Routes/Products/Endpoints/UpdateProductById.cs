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
    public int Id { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal? PriceReference { get; set; }
    public bool? IsVisible { get; set; }
}

public static class UpdateProductById
{
    public static async
        Task<Results<Ok<ApiResponse<Product>>,
            NotFound<ApiResponse<Product>>,
            BadRequest<ApiResponse<Product>>,
            InternalServerError<ApiResponse<Product>>>> 
        Handle(ApplicationDbContext db,
            ProductUpdateRequest productRequest,
            int id)
    {
        try
        {
            if (productRequest.Id != id)
            {
                return TypedResults.BadRequest(
                    ApiResponse.Fail<Product>("Invalid Product id"));
            }
            
            var productFromDb = await db.Products.FindAsync(id);
            
            if (productFromDb == null)
            {
                return TypedResults.NotFound(
                    ApiResponse.Fail<Product>("Product not found", HttpStatusCode.NotFound));
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
            if (!db.ChangeTracker.HasChanges())
            {
                return TypedResults.BadRequest(
                    ApiResponse.Fail<Product>("No changes detected in submitted data"));
            }

            await db.SaveChangesAsync();
            
            return TypedResults.Ok(
                ApiResponse.Success<Product>());
        }
        catch (Exception exception)
        {
            return TypedResults.InternalServerError(
                ApiResponse.Fail<Product>(exception.Message, HttpStatusCode.InternalServerError));
        }
    }
}