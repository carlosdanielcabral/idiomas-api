using System.Net;
using IdiomasAPI.Source.Application.Error;
using IdiomasAPI.Source.Domain.Enum;
using IdiomasAPI.Source.Interface.Repository;

namespace IdiomasAPI.Source.Application.UseCase.File;

public class FailFileUpload(IFileRepository fileRepository)
{
    private readonly IFileRepository _fileRepository = fileRepository;

    public async Task Execute(string fileKey, string userId)
    {
        var file = await this._fileRepository.GetByKey(fileKey);
        
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
        
        await this._fileRepository.ChangeStatus(fileKey, FileStatus.Failed);
    }
}   