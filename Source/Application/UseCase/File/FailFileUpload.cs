using System.Net;
using IdiomasAPI.Source.Application.Error;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Domain.Enum;
using IdiomasAPI.Source.Interface.Repository;

namespace IdiomasAPI.Source.Application.UseCase.File;

public class FailFileUpload(IFileRepository fileRepository)
{
    private readonly IFileRepository _fileRepository = fileRepository;

    public async Task Execute(string filekey, string userId)
    {
        await this.ValidateFile(filekey, userId);
        await this._fileRepository.ChangeStatus(filekey, FileStatus.Failed);
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
            throw new ApiException("Você não está autorizado a confirmar a falha no upload deste arquivo", HttpStatusCode.Unauthorized);
        }

        if (file.Status != FileStatus.Pending)
        {
            throw new ApiException("Arquivo já foi processado", HttpStatusCode.Conflict);
        }
    }
}   