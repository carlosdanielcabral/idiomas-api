using System.Net;

namespace Idiomas.Core.Exceptions.Validation;

public sealed class StringTooShortException(string fieldName, int minimumLength) : ApiException(
    errorCode: "validation:string-too-short",
    title: "String too short",
    statusCode: HttpStatusCode.BadRequest,
    detail: $"The field '{fieldName}' must be at least {minimumLength} characters long.")
{
}
