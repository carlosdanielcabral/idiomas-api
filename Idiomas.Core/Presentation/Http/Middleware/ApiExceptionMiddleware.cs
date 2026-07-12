using System.Net;
using System.Text.Json;
using Idiomas.Core.Application.Error;
using Microsoft.AspNetCore.Mvc;

namespace Idiomas.Core.Presentation.Http.Middleware;

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
        catch (ApiException exception)
        {
            _logger.LogWarning(exception, "API exception: {ErrorCode}", exception.ErrorCode);

            await HandleApiExceptionAsync(context, exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Internal server error.");

            await HandleUnhandledExceptionAsync(context);
        }
    }

    private static Task HandleApiExceptionAsync(HttpContext context, ApiException exception)
    {
        ProblemDetails problem = new()
        {
            Type = ProblemDetailsUris.ErrorType(exception.ErrorCode),
            Title = exception.Title,
            Status = (int)exception.StatusCode,
            Detail = exception.Detail,
            Instance = ProblemDetailsUris.Instance(context.TraceIdentifier)
        };

        foreach (KeyValuePair<string, object?> extension in exception.Extensions)
        {
            problem.Extensions[extension.Key] = extension.Value;
        }

        return WriteResponseAsync(context, (int)exception.StatusCode, problem);
    }

    private static Task HandleUnhandledExceptionAsync(HttpContext context)
    {
        ProblemDetails problem = new()
        {
            Type = "about:blank",
            Title = "Internal Server Error",
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = ProblemDetailsUris.Instance(context.TraceIdentifier)
        };

        return WriteResponseAsync(context, (int)HttpStatusCode.InternalServerError, problem);
    }

    private static Task WriteResponseAsync(HttpContext context, int statusCode, ProblemDetails problem)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        string result = JsonSerializer.Serialize(problem);

        return context.Response.WriteAsync(result);
    }
}
