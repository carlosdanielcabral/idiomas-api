using System.Net;
using Idiomas.Core.Application.Error.File;
using Idiomas.Core.Application.UseCase.File;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.File;

public class FailFileUploadTest
{
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly FailFileUpload _sut;

    public FailFileUploadTest()
    {
        this._fileRepositoryMock = new Mock<IFileRepository>();
        this._sut = new FailFileUpload(_fileRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldChangeFileStatusToFailed_WhenFileIsValid()
    {
        string fileKey = "valid-file-key";
        string userId = "user-id-123";

        CFile file = new("file-id", "test.jpg", fileKey, "image/jpeg", 1024, userId, FileStatus.Pending);

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync(file);

        await this._sut.Execute(fileKey, userId);

        this._fileRepositoryMock.Verify(repository => repository.ChangeStatus(fileKey, FileStatus.Failed), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowFileNotFoundException_WhenFileKeyDoesNotExist()
    {
        string fileKey = "non-existing-key";
        string userId = "user-id-123";

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync((CFile) null!);

        var exception = await Assert.ThrowsAsync<FileUploadNotFoundException>(() => this._sut.Execute(fileKey, userId));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("file:not-found", exception.ErrorCode);
        Assert.Equal("File not found", exception.Title);
        Assert.Equal("The requested file was not found.", exception.Detail);

        this._fileRepositoryMock.Verify(repository => repository.ChangeStatus(It.IsAny<string>(), It.IsAny<FileStatus>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowFileAccessDeniedException_WhenUserIsNotTheOwner()
    {
        string fileKey = "valid-file-key";
        string ownerId = "owner-id-123";
        string attackerId = "attacker-id-456";

        CFile file = new("file-id", "test.jpg", fileKey, "image/jpeg", 1024, ownerId, FileStatus.Pending);

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync(file);

        var exception = await Assert.ThrowsAsync<FileAccessDeniedException>(() => this._sut.Execute(fileKey, attackerId));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("file:access-denied", exception.ErrorCode);
        Assert.Equal("File access denied", exception.Title);
        Assert.Equal("You are not authorized to perform this action on the file.", exception.Detail);

        this._fileRepositoryMock.Verify(repository => repository.ChangeStatus(It.IsAny<string>(), It.IsAny<FileStatus>()), Times.Never);
    }

    [Theory]
    [InlineData(FileStatus.Uploaded)]
    [InlineData(FileStatus.Failed)]
    public async Task Execute_ShouldThrowFileAlreadyProcessedException_WhenFileStatusIsNotPending(FileStatus initialStatus)
    {
        string fileKey = "valid-file-key";
        string userId = "user-id-123";

        CFile file = new("file-id", "test.jpg", fileKey, "image/jpeg", 1024, userId, initialStatus);

        this._fileRepositoryMock
            .Setup(repository => repository.GetByKey(fileKey))
            .ReturnsAsync(file);

        var exception = await Assert.ThrowsAsync<FileAlreadyProcessedException>(() => this._sut.Execute(fileKey, userId));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal("file:already-processed", exception.ErrorCode);
        Assert.Equal("File already processed", exception.Title);
        Assert.Equal("The file has already been processed.", exception.Detail);

        this._fileRepositoryMock.Verify(repository => repository.ChangeStatus(It.IsAny<string>(), It.IsAny<FileStatus>()), Times.Never);
    }
}
