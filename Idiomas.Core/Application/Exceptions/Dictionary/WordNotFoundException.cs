using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Dictionary;

public sealed class WordNotFoundException() : ApiException(
    errorCode: "dictionary:word-not-found",
    title: "Word not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested word was not found.")
{
}
