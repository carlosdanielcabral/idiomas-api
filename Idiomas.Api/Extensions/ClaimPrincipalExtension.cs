using System.Security.Claims;

namespace Idiomas.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        string? userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new ApplicationException("User ID claim is missing or invalid.");
        }

        return userId;
    }
}