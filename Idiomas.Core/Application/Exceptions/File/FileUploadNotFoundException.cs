using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.File;

public sealed class FileUploadNotFoundException() : ApiException(
    errorCode: "file:not-found",
    title: "File not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested file was not found.")
{
}
