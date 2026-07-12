using System.Net;

namespace Idiomas.Core.Application.Error.File;

public sealed class FileUploadNotFoundException() : ApiException(
    errorCode: "file:not-found",
    title: "File not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested file was not found.")
{
}
