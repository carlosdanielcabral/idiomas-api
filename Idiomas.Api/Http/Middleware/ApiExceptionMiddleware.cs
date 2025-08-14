using System.Net;
using System.Text.Json;
using Idiomas.Core.Application.Error;

namespace Idiomas.Api.Http.Middleware;

public class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ApiExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "API exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.StatusCode, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error.");
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, new { error = "Internal server error" });
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, object response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(result);
    }
}