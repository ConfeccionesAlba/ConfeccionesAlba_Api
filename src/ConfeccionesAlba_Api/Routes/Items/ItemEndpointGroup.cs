using ConfeccionesAlba_Api.Authorization;
using ConfeccionesAlba_Api.Extensions;
using ConfeccionesAlba_Api.Filters;
using ConfeccionesAlba_Api.Models.Dtos.Items;
using ConfeccionesAlba_Api.Routes.Items.Endpoints;

namespace ConfeccionesAlba_Api.Routes.Items;

public static class ItemEndpointGroup
{
    public static RouteGroupBuilder MapItemsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/items")
            .WithTags("Item Api")
            .WithOpenApi();
        
        group.MapGet("/", GetItems.Handle)
            .WithName(ItemsEndpointNames.GetItems)
            .WithSummary("Get all items");
        
        group.MapGet("/{id:int}", GetItemById.Handle)
            .WithName(ItemsEndpointNames.GetItemById)
            .WithSummary("Get item by Id");
        
        group.MapPost("/", CreateItem.Handle)
            .WithName(ItemsEndpointNames.CreateItem)
            .WithSummary("Create new item")
            .AddEndpointFilter<ValidationFilter<ItemCreateDto>>()
            .RequireAuthorization(policy => policy.RequirePermission(Permission.ItemCreate));

        group.MapPut("/{id:int}", UpdateItemById.Handle)
            .WithName(ItemsEndpointNames.UpdateItem)
            .WithSummary("Update item")
            .AddEndpointFilter<ValidationFilter<ItemUpdateDto>>()
            .RequireAuthorization(policy => policy.RequirePermission(Permission.ItemUpdate));
        
        return group;
    }
}

public static class ItemsEndpointNames
{
    public const string GetItems = "GetItems";
    public const string GetItemById = "GetItemById";
    public const string CreateItem = "CreateItem";
    public const string UpdateItem = "UpdateItem";
} 