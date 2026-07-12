using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Dictionary;

public sealed class WordAlreadyExistsException() : ApiException(
    errorCode: "dictionary:word-already-exists",
    title: "Word already exists",
    statusCode: HttpStatusCode.Conflict,
    detail: "A word with the same name already exists in your dictionary.")
{
}
