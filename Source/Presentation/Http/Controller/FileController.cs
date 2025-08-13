using System.Security.Claims;
using IdiomasAPI.Application.DTO.File;
using IdiomasAPI.Source.Application.DTO.File;
using IdiomasAPI.Source.Application.UseCase.File;
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Presentation.Extensions;

namespace IdiomasAPI.Source.Application.Http.Controller;

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