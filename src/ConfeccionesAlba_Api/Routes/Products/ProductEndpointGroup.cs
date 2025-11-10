using ConfeccionesAlba_Api.Authorization;
using ConfeccionesAlba_Api.Extensions;
using ConfeccionesAlba_Api.Filters;
using ConfeccionesAlba_Api.Routes.Products.Endpoints;

namespace ConfeccionesAlba_Api.Routes.Products;

public static class ProductEndpointGroup
{
    public static RouteGroupBuilder MapItemsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/products")
            .WithTags("Product Api")
            .WithOpenApi();
        
        group.MapGet("/", GetProducts.Handle)
            .WithName(ProductsEndpointNames.GetProducts)
            .WithSummary("Get all Products");
        
        group.MapGet("/{id:int}", GetProductById.Handle)
            .WithName(ProductsEndpointNames.GetProductById)
            .WithSummary("Get Product by Id");
        
        group.MapPost("/", CreateProduct.Handle)
            .WithName(ProductsEndpointNames.CreateProduct)
            .WithSummary("Create new Product")
            .AddEndpointFilter<ValidationFilter<ProductCreateRequest>>()
            .DisableAntiforgery()
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.ProductsCreate));

        group.MapPut("/{id:int}", UpdateProductById.Handle)
            .WithName(ProductsEndpointNames.UpdateProduct)
            .WithSummary("Update Product")
            .AddEndpointFilter<ValidationFilter<ProductUpdateRequest>>()
            .DisableAntiforgery()
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.ProductsUpdate));
        
        return group;
    }
}

public static class ProductsEndpointNames
{
    public const string GetProducts = "GetProducts";
    public const string GetProductById = "GetProductById";
    public const string CreateProduct = "CreateProduct";
    public const string UpdateProduct = "UpdateProduct";
} 