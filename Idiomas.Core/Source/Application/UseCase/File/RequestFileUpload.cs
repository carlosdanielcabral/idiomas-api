using Idiomas.Application.DTO.File;
using Idiomas.Source.Application.Mapper;
using Idiomas.Source.Helper;
using Idiomas.Source.Interface.Repository;
using Idiomas.Source.Interface.Storage;

namespace Idiomas.Source.Application.UseCase.File;

public record struct Response (string UrlToUpload, string FileKey);

public class RequestFileUpload(IFileRepository fileRepository, IFileStorage fileStorage, FileHelper fileHelper)
{
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly IFileStorage _fileStorage = fileStorage;
    private readonly FileHelper _fileHelper = fileHelper;

    public async Task<Response> Execute(CreateFileDTO dto, string userId)
    {
        string fileKey = this._fileHelper.GenerateFileKey(dto.OriginalFilename);
        string urlToUpload = await this._fileStorage.GenerateUrlToUpload(fileKey);

        await this._fileRepository.Insert(dto.ToEntity(fileKey, userId));

        return new Response() { UrlToUpload = urlToUpload, FileKey = fileKey };
    }
}