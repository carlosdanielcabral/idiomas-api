using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Dictionary;

public sealed class WordAccessDeniedException() : ApiException(
    errorCode: "dictionary:word-access-denied",
    title: "Word access denied",
    statusCode: HttpStatusCode.Forbidden,
    detail: "You do not have permission to modify this word.")
{
}
