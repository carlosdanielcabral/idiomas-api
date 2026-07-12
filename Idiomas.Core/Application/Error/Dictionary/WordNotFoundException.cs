using System.Net;

namespace Idiomas.Core.Application.Error.Dictionary;

public sealed class WordNotFoundException() : ApiException(
    errorCode: "dictionary:word-not-found",
    title: "Word not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested word was not found.")
{
}
