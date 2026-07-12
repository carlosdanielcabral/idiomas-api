using Google.Apis.Auth;
using Idiomas.Core.Application.Error.Infrastructure;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;

namespace Idiomas.Core.Infrastructure.Service.Google;

public class GoogleTokenVerifier(IConfiguration configuration) : IGoogleTokenVerifier
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<GoogleTokenPayload> Verify(string idToken)
    {
        string? clientId = this._configuration["Google:ClientId"];

        if (string.IsNullOrEmpty(clientId))
        {
            throw new GoogleConfigurationMissingException();
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [clientId]
        };

        try
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleTokenPayload(payload.Subject, payload.Email, payload.Name, payload.EmailVerified);
        }
        catch (Exception)
        {
            throw new GoogleTokenInvalidException();
        }
    }
}
