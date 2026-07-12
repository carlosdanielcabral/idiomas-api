using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.File;

public sealed class FileAccessDeniedException() : ApiException(
    errorCode: "file:access-denied",
    title: "File access denied",
    statusCode: HttpStatusCode.Unauthorized,
    detail: "You are not authorized to perform this action on the file.")
{
}
