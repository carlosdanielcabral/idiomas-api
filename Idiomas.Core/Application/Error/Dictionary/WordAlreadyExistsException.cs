using System.Net;

namespace Idiomas.Core.Application.Error.Dictionary;

public sealed class WordAlreadyExistsException() : ApiException(
    errorCode: "dictionary:word-already-exists",
    title: "Word already exists",
    statusCode: HttpStatusCode.Conflict,
    detail: "A word with the same name already exists in your dictionary.")
{
}
