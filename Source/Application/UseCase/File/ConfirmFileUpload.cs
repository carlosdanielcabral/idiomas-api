using System.Net;
using Idiomas.Source.Application.Error;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Domain.Enum;
using Idiomas.Source.Interface.Repository;

namespace Idiomas.Source.Application.UseCase.File;

public class ConfirmFileUpload(IFileRepository fileRepository)
{
    private readonly IFileRepository _fileRepository = fileRepository;

    public async Task Execute(string filekey, string userId)
    {
        await this.ValidateFile(filekey, userId);
        await this._fileRepository.ChangeStatus(filekey, FileStatus.Uploaded);
    }

    public async Task ValidateFile(string filekey, string userId)
    {
        CFile? file = await this._fileRepository.GetByKey(filekey);

        if (file is null)
        {
            throw new ApiException("Arquivo não encontrado", HttpStatusCode.NotFound);
        }

        if (file.UserId != userId)
        {
            throw new ApiException("Você não está autorizado a confirmar o upload deste arquivo", HttpStatusCode.Unauthorized);
        }

        if (file.Status != FileStatus.Pending)
        {
            throw new ApiException("Arquivo já foi processado", HttpStatusCode.Conflict);
        }
    }
}   