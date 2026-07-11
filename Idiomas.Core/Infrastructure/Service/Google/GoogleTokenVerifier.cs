using Google.Apis.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Infrastructure.Service.Google;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Idiomas.Core.Infrastructure.Service.Google;

public class GoogleTokenVerifier(IConfiguration configuration) : IGoogleTokenVerifier
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<GoogleTokenPayload> Verify(string idToken)
    {
        string? clientId = this._configuration["Google:ClientId"];

        if (string.IsNullOrEmpty(clientId))
        {
            throw new ApiException("Configuração do Google ausente", HttpStatusCode.InternalServerError);
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
            throw new ApiException("Token do Google inválido", HttpStatusCode.Unauthorized);
        }
    }
}
