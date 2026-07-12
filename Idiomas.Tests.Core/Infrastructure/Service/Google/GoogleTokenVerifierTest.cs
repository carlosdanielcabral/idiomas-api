using Idiomas.Core.Infrastructure.Exceptions.Google;
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
    public async Task Verify_ThrowsGoogleConfigurationMissingException_WhenClientIdIsMissing()
    {
        this._configurationMock
            .Setup(configuration => configuration["Google:ClientId"])
            .Returns((string?)null);

        var exception = await Assert.ThrowsAsync<GoogleConfigurationMissingException>(() => this._sut.Verify("any-token"));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Equal("infrastructure:google-configuration-missing", exception.ErrorCode);
        Assert.Equal("Google configuration missing", exception.Title);
        Assert.Equal("Google OAuth configuration is missing or incomplete.", exception.Detail);
    }

    [Fact]
    public async Task Verify_ThrowsGoogleConfigurationMissingException_WhenClientIdIsEmpty()
    {
        this._configurationMock
            .Setup(configuration => configuration["Google:ClientId"])
            .Returns("");

        var exception = await Assert.ThrowsAsync<GoogleConfigurationMissingException>(() => this._sut.Verify("any-token"));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task Verify_ThrowsGoogleTokenInvalidException_WhenTokenIsInvalid()
    {
        this._configurationMock
            .Setup(configuration => configuration["Google:ClientId"])
            .Returns("valid-client-id.apps.googleusercontent.com");

        var exception = await Assert.ThrowsAsync<GoogleTokenInvalidException>(() => this._sut.Verify("invalid-token"));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("infrastructure:google-token-invalid", exception.ErrorCode);
        Assert.Equal("Google token invalid", exception.Title);
        Assert.Equal("The provided Google token is invalid or has expired.", exception.Detail);
    }
}
