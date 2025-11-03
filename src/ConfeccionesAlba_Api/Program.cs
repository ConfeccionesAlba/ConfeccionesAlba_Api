using System.Text;
using ConfeccionesAlba_Api;
using ConfeccionesAlba_Api.Authorization;
using ConfeccionesAlba_Api.Configurations;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Extensions;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Auth;
using ConfeccionesAlba_Api.Routes.Auth.Services;
using ConfeccionesAlba_Api.Routes.Categories;
using ConfeccionesAlba_Api.Routes.Items;
using ConfeccionesAlba_Api.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Resolve the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Log the connection string (for development/debugging only, redact sensitive info in production)
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole(); // Or other logging providers
});

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Setup JwtBearer
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() ??
                  throw new InvalidOperationException("Missing configuration settings");

builder.Services.AddAuthentication(u =>
{
    u.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    u.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(u =>
{
    u.RequireHttpsMetadata = true;
    u.SaveToken = true;
    u.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
    };
});

builder.Services.AddAuthorization(options =>
{
    foreach (Permissions p in Enum.GetValues(typeof(Permissions)))
    {
        options.AddPolicy(p.ToName(), policy =>
        {
            policy.Requirements.Add(new PermissionAuthorizationRequirement(p.ToName()));
        });
    }
});

builder.Services.AddScoped<IClaimsTransformation, PermissionClaimsTransformation>();

builder.Services.AddCors();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Setup validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<TokenService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.EnsureNoPendingMigrationsOrFail();
    await services.EnsureAdminUserAndRole();
}

var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (app.Environment.IsDevelopment())
{
    // Logging the full string (use with caution)
    logger.LogInformation("Connection String: {ConnectionString}", connectionString);
}
else
{
    // Logging a redacted version (For Production)
    var redactedConnectionString = RedactSensitiveInfo(connectionString); // Implement this method
    logger.LogInformation("Redacted Connection String: {RedactedConnectionString}", redactedConnectionString);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Layout = ScalarLayout.Classic;
    });
}

app.UseHttpsRedirection();

// TODO: Update allowed origin after the Frontend is designed
app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*"));
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapCategoriesEndpoints();
app.MapItemsEndpoints();
app.MapAuthEndpoints();

await app.RunAsync();
return;

// Redact sensitive information by returning SHA256 hash of the connection string
string RedactSensitiveInfo(string connString)
{
    return string.IsNullOrEmpty(connString) ? connString : connString.GetSha256Hash();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}