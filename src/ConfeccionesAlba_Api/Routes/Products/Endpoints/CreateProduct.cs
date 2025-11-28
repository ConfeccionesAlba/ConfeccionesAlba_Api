using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfeccionesAlba_Api.Routes.Products.Endpoints;

public class ProductCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal PriceReference { get; set; }
    public bool IsVisible { get; set; }
}

public static class CreateProduct
{
    public static async Task<Results<
            CreatedAtRoute<ApiResponse<Product>>,
            BadRequest<ApiResponse<Product>>,
            InternalServerError<ApiResponse<Product>>>>
        Handle(ApplicationDbContext db, ProductCreateRequest productRequest)
    {
        var response = new ApiResponse<Product>();
        
        try
        {
            // Save to database
            var newProduct = new Product
            {
                Name = productRequest.Name,
                Description = productRequest.Description,
                CategoryId = productRequest.CategoryId,
                PriceReference = productRequest.PriceReference,
                IsVisible = productRequest.IsVisible,
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