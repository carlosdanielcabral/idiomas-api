using System.Security.Claims;
using IdiomasAPI.Application.DTO.File;
using IdiomasAPI.Source.Application.UseCase.File;

namespace IdiomasAPI.Source.Interface.Controller;

public interface IFileController
{
    public Task<IResult> GenerateUploadUrl(CreateFileDTO dto, ClaimsPrincipal user, RequestFileUpload useCase);
    public Task<IResult> ConfirmFileUpload(string fileKey, ClaimsPrincipal user, ConfirmFileUpload useCase);
    public Task<IResult> FailFileUpload(string fileKey, ClaimsPrincipal user, FailFileUpload useCase);

}
