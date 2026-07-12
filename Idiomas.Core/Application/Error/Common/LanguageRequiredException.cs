using System.Net;

namespace Idiomas.Core.Application.Error.Common;

public sealed class LanguageRequiredException() : ApiException(
    errorCode: "common:language-required",
    title: "Language required",
    statusCode: HttpStatusCode.BadRequest,
    detail: "A language must be specified.")
{
}
