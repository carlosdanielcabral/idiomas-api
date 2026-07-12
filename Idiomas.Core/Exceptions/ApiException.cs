using System.Net;

namespace Idiomas.Core.Exceptions;

public class ApiException(string errorCode, string title, HttpStatusCode statusCode, string? detail = null)
    : Exception(detail ?? title)
{
    public string ErrorCode { get; } = errorCode;
    public string Title { get; } = title;
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string? Detail { get; } = detail;
    public Dictionary<string, object?> Extensions { get; } = new();
}
