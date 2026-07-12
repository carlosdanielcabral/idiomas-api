using System.Net;

namespace Idiomas.Core.Application.Error.File;

public sealed class FileAccessDeniedException() : ApiException(
    errorCode: "file:access-denied",
    title: "File access denied",
    statusCode: HttpStatusCode.Unauthorized,
    detail: "You are not authorized to perform this action on the file.")
{
}
