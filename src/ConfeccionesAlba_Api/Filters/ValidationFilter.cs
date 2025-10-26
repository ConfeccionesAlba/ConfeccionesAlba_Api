using System.Net;
using ConfeccionesAlba_Api.Models;
using FluentValidation;

namespace ConfeccionesAlba_Api.Filters;

/// <summary>
///     A filter that validates incoming request models using FluentValidation.
///     This filter intercepts HTTP requests, validates the request model against
///     the provided validator, and returns appropriate responses based on validation results.
/// </summary>
/// <typeparam name="T">The type of the model to validate, which must be a class.</typeparam>
public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    /// <summary>
    ///     Invokes the validation filter asynchronously.
    /// </summary>
    /// <param name="context">The endpoint filter invocation context containing request information and arguments.</param>
    /// <param name="next">The delegate representing the next filter or endpoint in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation. The result is the response object.</returns>
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