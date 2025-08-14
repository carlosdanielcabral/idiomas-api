using System.Security.Claims;
using Idiomas.Core.Application.DTO.File;
using Idiomas.Core.Application.DTO.File;
using Idiomas.Core.Application.UseCase.File;
using Idiomas.Api.Interface.Controller;
using Idiomas.Api.Extensions;

namespace Idiomas.Api.Http.Controller;

public class FileController : IFileController
{
    public async Task<IResult> GenerateUploadUrl(CreateFileDTO dto, ClaimsPrincipal user, RequestFileUpload useCase)
    {
        var result = await useCase.Execute(dto, user.GetUserId().ToString());

        CreateFileResponseDTO response = new(result.UrlToUpload, result.FileKey);

        return TypedResults.Ok(response);
    }

    public async Task<IResult> ConfirmFileUpload(string fileKey, ClaimsPrincipal user, ConfirmFileUpload useCase)
    {
        await useCase.Execute(fileKey, user.GetUserId().ToString());

        return TypedResults.NoContent();
    }

    public async Task<IResult> FailFileUpload(string fileKey, ClaimsPrincipal user, FailFileUpload useCase)
    {
        await useCase.Execute(fileKey, user.GetUserId().ToString());

        return TypedResults.NoContent();
    }
}