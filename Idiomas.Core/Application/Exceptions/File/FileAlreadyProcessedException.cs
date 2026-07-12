using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.File;

public sealed class FileAlreadyProcessedException() : ApiException(
    errorCode: "file:already-processed",
    title: "File already processed",
    statusCode: HttpStatusCode.Conflict,
    detail: "The file has already been processed.")
{
}
