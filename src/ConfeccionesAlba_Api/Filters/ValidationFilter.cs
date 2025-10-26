using System.Net;
using ConfeccionesAlba_Api.Models;
using FluentValidation;

namespace ConfeccionesAlba_Api.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();

        if (model is null)
        {
            return await next(context);
        }
        
        var validationResult = await validator.ValidateAsync(model);

        if (validationResult.IsValid)
        {
            return await next(context);
        }
        
        var response = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.BadRequest
        };
        response.ErrorMessages.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

        return TypedResults.BadRequest(response);

    }
}