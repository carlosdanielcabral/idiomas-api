using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Service;
using Microsoft.IdentityModel.Tokens;

namespace Idiomas.Core.Infrastructure.Service.Authentication;

public class JWT(IConfiguration configuration) : IToken
{
    private readonly IConfiguration _configuration = configuration;

    public string Generate(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        
        byte[] key = Encoding.UTF8.GetBytes(this._configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured."));

        Claim[] claims = {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}