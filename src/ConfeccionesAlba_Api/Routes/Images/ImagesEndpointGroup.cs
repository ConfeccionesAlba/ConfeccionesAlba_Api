using ConfeccionesAlba_Api.Authorization;
using ConfeccionesAlba_Api.Extensions;
using ConfeccionesAlba_Api.Routes.Images.Endpoints;

namespace ConfeccionesAlba_Api.Routes.Images;

public static class ImagesEndpointGroup
{
    public static RouteGroupBuilder MapItemsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/images")
            .WithTags("Image Api")
            .WithOpenApi();

        group.MapGet("/", GetImages.Handle)
            .WithName(ImagesEndpointNames.GetImages)
            .WithSummary("Get images");
        
        group.MapGet("/{id:int}", GetImageById.Handle)
            .WithName(ImagesEndpointNames.GetImageById)
            .WithSummary("Get image by id");
        
        // TODO: Check if validation is needed
        group.MapPost("/", CreateImage.Handle)
            .WithName(ImagesEndpointNames.CreateImage)
            .WithSummary("Create new image")
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.ImagesCreate));

        group.MapDelete("/{id:int}", DeleteImageById.Handle)
            .WithName(ImagesEndpointNames.DeleteImage)
            .WithSummary("Delete image")
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.ImagesDelete));
        
        return group;
    }
}

public static class ImagesEndpointNames
{
    public const string GetImages = "GetImages";
    public const string GetImageById = "GetImageById";
    public const string CreateImage = "CreateImage";
    public const string DeleteImage = "UpdateImage";
} 