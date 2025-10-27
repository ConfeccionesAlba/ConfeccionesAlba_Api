using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Routes.Categories;
using ConfeccionesAlba_Api.Routes.Items;
using ConfeccionesAlba_Api.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Setup validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

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