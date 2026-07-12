using System.Net;

namespace Idiomas.Core.Application.Error.Validation;

public sealed class FieldRequiredException(string fieldName) : ApiException(
    errorCode: "validation:field-required",
    title: "Field required",
    statusCode: HttpStatusCode.BadRequest,
    detail: $"The field '{fieldName}' is required.")
{
}
