using System.Net;

namespace Idiomas.Core.Exceptions.Validation;

public sealed class ItemAtPositionRequiredException(string fieldName, int position) : ApiException(
    errorCode: "validation:item-at-position-required",
    title: "Item at position required",
    statusCode: HttpStatusCode.BadRequest,
    detail: $"The field '{fieldName}' at position {position} is required.")
{
}
