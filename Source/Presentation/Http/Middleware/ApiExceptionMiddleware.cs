using System.Net;
using System.Text.Json;
using IdiomasAPI.Source.Application.Error;

namespace IdiomasAPI.Source.Presentation.Http.Middleware;

public class ApiExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            context.Response.StatusCode = (int) ex.StatusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = ex.Message });
    
            await context.Response.WriteAsync(result);
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = "Internal server error" });

            await context.Response.WriteAsync(result);
        }
    }
}
