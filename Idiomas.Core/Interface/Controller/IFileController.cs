using System.Security.Claims;
using Idiomas.Application.DTO.File;
using Idiomas.Core.Application.UseCase.File;
using Idiomas.Core.Presentation.Http.Validator.File;

namespace Idiomas.Core.Interface.Controller;

public interface IFileController
{
    public Task<IResult> GenerateUploadUrl(CreateFileDTO dto, ClaimsPrincipal user, RequestFileUploadValidator validator, RequestFileUpload useCase);
    public Task<IResult> ConfirmFileUpload(string fileKey, ClaimsPrincipal user, ConfirmFileUpload useCase);
    public Task<IResult> FailFileUpload(string fileKey, ClaimsPrincipal user, FailFileUpload useCase);
}
