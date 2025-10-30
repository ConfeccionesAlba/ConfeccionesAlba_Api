using ConfeccionesAlba_Api.Filters;
using ConfeccionesAlba_Api.Models.Dtos.Auth.Validators;
using ConfeccionesAlba_Api.Routes.Auth.Endpoints;

namespace ConfeccionesAlba_Api.Routes.Auth;

public static class AuthEndpointGroup
{
    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/auth")
            .WithTags("Authentication Api")
            .WithOpenApi();
        
        group.MapPost("/login", LoginUser.Handle)
            .WithName(AuthEndpointNames.Login)
            .WithSummary("Login user")
            .AddEndpointFilter<ValidationFilter<LoginRequestDtoValidator>>();

        group.MapPost("/register", RegisterUser.Handle)
            .WithName(AuthEndpointNames.Register)
            .WithSummary("Register user")
            .AddEndpointFilter<ValidationFilter<RegisterRequestDtoValidator>>();
        
        return group;
    }
}

public static class AuthEndpointNames
{
    public const string Login = "Login";
    public const string Register = "Register";
} 