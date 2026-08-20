using FluentValidation;
using System.Net;
using System.Text.Json;
using TaskManagement.API.Responses;

namespace TaskManagement.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
{
    ArgumentNullException.ThrowIfNull(context);

    try
    {
        await _next(context);
    }
    catch (ValidationException ex)
    {
        await HandleValidationException(context, ex);
    }
    catch (UnauthorizedAccessException ex)
    {
        await HandleException(
            context,
            HttpStatusCode.Unauthorized,
            ex.Message);
    }
    catch (KeyNotFoundException ex)
    {
        await HandleException(
            context,
            HttpStatusCode.NotFound,
            ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception occurred.");

        await HandleException(
            context,
            HttpStatusCode.InternalServerError,
            "An unexpected error occurred.");
    }
}
    private static async Task HandleValidationException(
        HttpContext context,
        ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var response = new ValidationErrorResponse
{
    StatusCode = StatusCodes.Status400BadRequest,
    Message = "Validation Failed",
    Errors = exception.Errors
        .GroupBy(x => x.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray())
};

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    private static async Task HandleException(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}