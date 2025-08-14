using System.Security.Claims;
using Idiomas.Application.DTO.File;
using Idiomas.Source.Application.UseCase.File;

namespace Idiomas.Source.Interface.Controller;

public interface IFileController
{
    public Task<IResult> GenerateUploadUrl(CreateFileDTO dto, ClaimsPrincipal user, RequestFileUpload useCase);
    public Task<IResult> ConfirmFileUpload(string fileKey, ClaimsPrincipal user, ConfirmFileUpload useCase);
    public Task<IResult> FailFileUpload(string fileKey, ClaimsPrincipal user, FailFileUpload useCase);

}
