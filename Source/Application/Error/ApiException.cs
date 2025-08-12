using System.Net;

namespace IdiomasAPI.Source.Application.Error;

public class ApiException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
