using System.Net;

namespace Idiomas.Core.Exceptions.Validation;

public sealed class NumberMustBePositiveException(string fieldName) : ApiException(
    errorCode: "validation:number-must-be-positive",
    title: "Number must be positive",
    statusCode: HttpStatusCode.BadRequest,
    detail: $"The field '{fieldName}' must be greater than zero.")
{
}
