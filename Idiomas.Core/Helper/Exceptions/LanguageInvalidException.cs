using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Helper.Error;

public sealed class LanguageInvalidException(string language, string availableLanguages) : ApiException(
    errorCode: "common:language-invalid",
    title: "Language invalid",
    statusCode: HttpStatusCode.BadRequest,
    detail: $"Invalid language '{language}'. Available languages: {availableLanguages}")
{
}
