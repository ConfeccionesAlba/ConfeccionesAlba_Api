using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace ConfeccionesAlba_Api;

internal sealed class BearerSecuritySchemeTransformer(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
        {
            var requirement = new Dictionary<string, OpenApiSecurityScheme> // TODO: Move to IOpenApiSecurityScheme after upgrade to Net 10
            {
                [JwtBearerDefaults.AuthenticationScheme] = new()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirement;
        }

        document.Info = new OpenApiInfo
        {
            Title = "Confecciones Alba API",
            Version = "v1",
            Description = "Confecciones artesanales",
        };
    }
}