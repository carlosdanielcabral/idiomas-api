using System.Net;

namespace Idiomas.Core.Application.Error;

public class ApiException(string errorCode, string title, HttpStatusCode statusCode, string? detail = null)
    : Exception(detail ?? title)
{
    public string ErrorCode { get; } = errorCode;
    public string Title { get; } = title;
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string? Detail { get; } = detail;
    public Dictionary<string, object?> Extensions { get; } = new();

    // Legacy constructor: kept during migration so existing throw sites compile.
    // Removed in Task 12.
    public ApiException(string message, HttpStatusCode statusCode)
        : this("about:blank", message, statusCode, message)
    {
    }
}
