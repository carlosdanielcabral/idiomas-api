using System.Net;

namespace Idiomas.Core.Application.Error.Dictionary;

public sealed class WordAccessDeniedException() : ApiException(
    errorCode: "dictionary:word-access-denied",
    title: "Word access denied",
    statusCode: HttpStatusCode.Forbidden,
    detail: "You do not have permission to modify this word.")
{
}
