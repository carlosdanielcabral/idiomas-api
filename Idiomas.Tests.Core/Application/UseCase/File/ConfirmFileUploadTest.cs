using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.File;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.File;

public class ConfirmFileUploadTest
{
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly ConfirmFileUpload _sut;

    public ConfirmFileUploadTest()
    {
        this._fileRepositoryMock = new Mock<IFileRepository>();
        this._sut = new ConfirmFileUpload(_fileRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldChangeFileStatusToUploaded_WhenFileIsValid()
    {
        string fileKey = "valid-file-key";
        string userId = "user-id-123";

        CFile file = new("file-id", "test.jpg", fileKey, "image/jpeg", 1024, userId, FileStatus.Pending);

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync(file);

        await this._sut.Execute(fileKey, userId);

        this._fileRepositoryMock.Verify(repository => repository.ChangeStatus(fileKey, FileStatus.Uploaded), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowNotFound_WhenFileKeyDoesNotExist()
    {
        string fileKey = "non-existing-key";
        string userId = "user-id-123";

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync((CFile) null!);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(fileKey, userId));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Arquivo não encontrado", exception.Message);

        this._fileRepositoryMock.Verify(repo => repo.ChangeStatus(It.IsAny<string>(), It.IsAny<FileStatus>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowUnauthorized_WhenUserIsNotTheOwner()
    {
        string fileKey = "valid-file-key";
        string ownerId = "owner-id-123";
        string attackerId = "attacker-id-456";

        CFile file = new("file-id", "test.jpg", fileKey, "image/jpeg", 1024, ownerId, FileStatus.Pending);

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync(file);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(fileKey, attackerId));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("Você não está autorizado a confirmar o upload deste arquivo", exception.Message);

        this._fileRepositoryMock.Verify(repo => repo.ChangeStatus(It.IsAny<string>(), It.IsAny<FileStatus>()), Times.Never);
    }

    [Theory]
    [InlineData(FileStatus.Uploaded)]
    [InlineData(FileStatus.Failed)]
    public async Task Execute_ShouldThrowConflict_WhenFileStatusIsNotPending(FileStatus initialStatus)
    {
        string fileKey = "valid-file-key";
        string userId = "user-id-123";

        CFile file = new("file-id", "test.jpg", fileKey, "image/jpeg", 1024, userId, initialStatus);

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync(file);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(fileKey, userId));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal("Arquivo já foi processado", exception.Message);

        this._fileRepositoryMock.Verify(repository => repository.ChangeStatus(It.IsAny<string>(), It.IsAny<FileStatus>()), Times.Never);
    }
}