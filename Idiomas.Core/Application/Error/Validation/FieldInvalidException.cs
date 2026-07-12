using System.Net;

namespace Idiomas.Core.Application.Error.Validation;

public sealed class FieldInvalidException(string fieldName) : ApiException(
    errorCode: "validation:field-invalid",
    title: "Field invalid",
    statusCode: HttpStatusCode.BadRequest,
    detail: $"The field '{fieldName}' contains an invalid value.")
{
}
