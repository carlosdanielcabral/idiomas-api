using Idiomas.Application.DTO.File;
using Idiomas.Core.Application.UseCase.File;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Helper;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Storage;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.File;

public class RequestFileUploadTest
{
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly Mock<IFileStorage> _fileStorageMock;
    private readonly FileHelper _fileHelper;
    private readonly RequestFileUpload _sut;

    public RequestFileUploadTest()
    {
        this._fileRepositoryMock = new Mock<IFileRepository>();
        this._fileStorageMock = new Mock<IFileStorage>();
        this._fileHelper = new FileHelper();
        this._sut = new RequestFileUpload(
            _fileRepositoryMock.Object,
            _fileStorageMock.Object,
            _fileHelper
        );
    }

    [Fact]
    public async Task Execute_Should_ReturnCorrectUrlAndKey_And_SaveFileMetadata()
    {
        CreateFileDTO createFileDto = new("test-file.jpg", "image/jpeg", 1024);

        string userId = "user-id-123";
        string expectedUploadUrl = "http://storage.com/upload-url";
        string generatedFileKey = string.Empty;

        this._fileStorageMock
            .Setup(storage => storage.GenerateUrlToUpload(It.IsAny<string>()))
            .Callback<string>(key => generatedFileKey = key)
            .ReturnsAsync(expectedUploadUrl);

        this._fileRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<CFile>()))
            .ReturnsAsync((CFile file) => file);

        var result = await this._sut.Execute(createFileDto, userId);

        Assert.Equal(expectedUploadUrl, result.UrlToUpload);
        Assert.Equal(generatedFileKey, result.FileKey);
        Assert.NotNull(result.FileKey);
        Assert.NotEmpty(result.FileKey);

        this._fileStorageMock.Verify(storage => storage.GenerateUrlToUpload(generatedFileKey), Times.Once);
        this._fileRepositoryMock.Verify(repository => repository.Insert(It.Is<CFile>(
            file => file.Key == generatedFileKey &&
                 file.OriginalName == createFileDto.OriginalFilename &&
                 file.UserId == userId &&
                 file.MimeType == createFileDto.MimeType &&
                 file.Size == createFileDto.Size
        )), Times.Once);
    }
}