using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Helper.Error;

public sealed class LanguageRequiredException() : ApiException(
    errorCode: "common:language-required",
    title: "Language required",
    statusCode: HttpStatusCode.BadRequest,
    detail: "A language must be specified.")
{
}
