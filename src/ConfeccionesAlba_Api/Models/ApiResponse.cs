using System.Net;

namespace ConfeccionesAlba_Api.Models;

public class ApiResponse<T> where T : class
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;
    public List<string> ErrorMessages { get; set; } = [];
    public T? Result { get; set; }
    
    private ApiResponse() { }

    public static ApiResponse<T> Success(
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            StatusCode = statusCode,
            IsSuccess = true
        };
    
    public static ApiResponse<T> Success(
        T result,
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            StatusCode = statusCode,
            Result = result,
            IsSuccess = true
        };

    public static ApiResponse<T> Fail(
        string errorMessage,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            StatusCode = statusCode,
            IsSuccess = false,
            ErrorMessages = [errorMessage]
        };

    public static ApiResponse<T> Fail(
        IEnumerable<string> errors,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            StatusCode = statusCode,
            IsSuccess = false,
            ErrorMessages = errors.ToList()
        };
}

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(
        T result,
        HttpStatusCode statusCode = HttpStatusCode.OK
    ) where T : class
        => ApiResponse<T>.Success(result, statusCode);

    public static ApiResponse<T> Success<T>(HttpStatusCode statusCode = HttpStatusCode.OK) where T : class
        => ApiResponse<T>.Success(statusCode);

    public static ApiResponse<T> Fail<T>(
        string errorMessage,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest
    ) where T : class
        => ApiResponse<T>.Fail(errorMessage, statusCode);

    public static ApiResponse<T> Fail<T>(
        IEnumerable<string> errors,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest
    ) where T : class
        => ApiResponse<T>.Fail(errors, statusCode);
}