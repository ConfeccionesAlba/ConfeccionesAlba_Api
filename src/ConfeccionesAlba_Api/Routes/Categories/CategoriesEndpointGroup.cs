using ConfeccionesAlba_Api.Authorization;
using ConfeccionesAlba_Api.Extensions;
using ConfeccionesAlba_Api.Filters;
using ConfeccionesAlba_Api.Models.Dtos.Categories;
using ConfeccionesAlba_Api.Routes.Categories.Endpoints;

namespace ConfeccionesAlba_Api.Routes.Categories;

public static class CategoriesEndpointGroup
{
    public static RouteGroupBuilder MapCategoriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/categories")
            .WithTags("Category Api")
            .WithOpenApi();
        
        group.MapGet("/", GetCategories.Handle)
            .WithName(CategoriesEndpointNames.GetCategories)
            .WithSummary("Get all categories");
        
        group.MapGet("/{id:int}", GetCategoryById.Handle)
            .WithName(CategoriesEndpointNames.GetCategoryById)
            .WithSummary("Get category by Id");

        group.MapPost("/", CreateCategory.Handle)
            .WithName(CategoriesEndpointNames.CreateCategory)
            .WithSummary("Create new category")
            .AddEndpointFilter<ValidationFilter<CategoryCreateDto>>()
            .RequireAuthorization(policy => policy.RequirePermission(Permission.CategoryCreate));
        
        group.MapPut("/{id:int}", UpdateCategoryById.Handle)
            .WithName(CategoriesEndpointNames.UpdateCategory)
            .WithSummary("Update category")
            .AddEndpointFilter<ValidationFilter<CategoryUpdateDto>>()
            .RequireAuthorization(policy => policy.RequirePermission(Permission.CategoryUpdate));
        
        return group;
    }
}

public static class CategoriesEndpointNames
{
    public const string GetCategories = "GetCategories";
    public const string GetCategoryById = "GetCategoryById";
    public const string CreateCategory = "CreateCategory";
    public const string UpdateCategory = "UpdateCategory";
} 