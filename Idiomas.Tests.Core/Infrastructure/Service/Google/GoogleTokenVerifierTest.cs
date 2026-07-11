using Idiomas.Core.Application.Error;
using Idiomas.Core.Infrastructure.Service.Google;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;

namespace Idiomas.Tests.Core.Infrastructure.Service.Google;

public class GoogleTokenVerifierTest
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly GoogleTokenVerifier _sut;

    public GoogleTokenVerifierTest()
    {
        this._configurationMock = new Mock<IConfiguration>();
        this._sut = new GoogleTokenVerifier(this._configurationMock.Object);
    }

    [Fact]
    public async Task Verify_ThrowsApiExceptionWhenClientIdIsMissing()
    {
        this._configurationMock
            .Setup(configuration => configuration["Google:ClientId"])
            .Returns((string?)null);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Verify("any-token"));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Equal("Configuração do Google ausente", exception.Message);
    }

    [Fact]
    public async Task Verify_ThrowsApiExceptionWhenClientIdIsEmpty()
    {
        this._configurationMock
            .Setup(configuration => configuration["Google:ClientId"])
            .Returns("");

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Verify("any-token"));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task Verify_ThrowsApiExceptionWhenTokenIsInvalid()
    {
        this._configurationMock
            .Setup(configuration => configuration["Google:ClientId"])
            .Returns("valid-client-id.apps.googleusercontent.com");

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Verify("invalid-token"));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("Token do Google inválido", exception.Message);
    }
}
