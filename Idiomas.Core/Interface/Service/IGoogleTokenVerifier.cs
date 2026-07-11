using Idiomas.Core.Infrastructure.Service.Google;

namespace Idiomas.Core.Interface.Service;

public interface IGoogleTokenVerifier
{
    Task<GoogleTokenPayload> Verify(string idToken);
}
