using System.Net;

namespace Idiomas.Core.Application.Error.Validation;

public sealed class StringTooLongException(string fieldName, int maximumLength) : ApiException(
    errorCode: "validation:string-too-long",
    title: "String too long",
    statusCode: HttpStatusCode.BadRequest,
    detail: $"The field '{fieldName}' must not exceed {maximumLength} characters.")
{
}
