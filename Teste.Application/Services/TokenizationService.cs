using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Teste.Application.Services.Implementations;
using Teste.Shared.Utilities;

namespace Teste.Application.Services;

public class TokenizationService(IConfiguration configuration) : ITokenizationImp
{
    public Task<(string, string)> GenerateTokenAsync(Guid accountId)
    {
        var expiry = DateTime.UtcNow.Add(TimeSpan.Parse(configuration.GetConfiguration<string>("Jwt:Expiry"),
            CultureInfo.InvariantCulture));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration.GetConfiguration<string>("Jwt:Secret"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Sid, accountId.ToString())
            ]),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = expiry,
            Issuer = configuration.GetConfiguration<string>("Jwt:Issuer"),
            Audience = configuration.GetConfiguration<string>("Jwt:Audience"),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult((tokenHandler.WriteToken(token), expiry.ToString("o")));
    }
}