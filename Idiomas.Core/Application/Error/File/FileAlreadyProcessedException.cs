using System.Net;

namespace Idiomas.Core.Application.Error.File;

public sealed class FileAlreadyProcessedException() : ApiException(
    errorCode: "file:already-processed",
    title: "File already processed",
    statusCode: HttpStatusCode.Conflict,
    detail: "The file has already been processed.")
{
}
