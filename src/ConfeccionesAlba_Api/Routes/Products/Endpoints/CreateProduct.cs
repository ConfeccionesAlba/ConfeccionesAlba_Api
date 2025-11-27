using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public record ProductCreateRequest(
    [FromForm] string Name,
    [FromForm] string Description,
    [FromForm] int CategoryId,
    [FromForm] decimal PriceReference,
    [FromForm] bool IsVisible,
    [FromForm] IFormFile File);

public static class CreateProduct
{
    public static async Task<Results<CreatedAtRoute<ApiResponse<Product>>, BadRequest<ApiResponse<Product>>, InternalServerError<ApiResponse<Product>>>> Handle(ApplicationDbContext db, IImageProcessor imageProcessor, [FromForm] ProductCreateRequest productRequest)
    {
        var response = new ApiResponse<Product>();
        
        try
        {
            // Get file
            var file = productRequest.File;
            
            var newFileName = $"{Guid.NewGuid()}.webp"; // TODO: Extract file extension from here
            
            var url = await imageProcessor.ProcessAsync(newFileName, file.ContentType, file.OpenReadStream());

            // Save to database
            var image = new Image { Name = newFileName, Url = url };

            var newProduct = new Product
            {
                Name = productRequest.Name,
                Description = productRequest.Description,
                CategoryId = productRequest.CategoryId,
                PriceReference = productRequest.PriceReference,
                IsVisible = productRequest.IsVisible,
                Image = image,
            };

            db.Products.Add(newProduct);
            await db.SaveChangesAsync();

            response.Result = newProduct;
            response.StatusCode = HttpStatusCode.Created;

            return TypedResults.CreatedAtRoute(response, ProductsEndpointNames.GetProductById, new { newProduct.Id });
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