using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Teste.Application.Services.Implementations;
using Teste.Domain.Enums;
using Teste.Shared.Utilities;

namespace Teste.Application.Services;

public class TokenizationService(IConfiguration configuration) : ITokenizationImp
{
    public async Task<(string, DateTime)> GenerateTokenAsync(Guid accountId, Role role)
    {
        var expiry = DateTime.UtcNow.Add(TimeSpan.Parse(configuration.GetConfiguration<string>("Jwt:Expiry"),
            CultureInfo.InvariantCulture));

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>
            {
                [ClaimTypes.PrimarySid] = accountId,
                [ClaimTypes.Role] = role.ToString()
            },
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = expiry,
            Issuer = configuration.GetConfiguration<string>("Jwt:Issuer"),
            Audience = configuration.GetConfiguration<string>("Jwt:Audience"),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetConfiguration<string>("Jwt:Secret"))),
                SecurityAlgorithms.HmacSha256Signature)
        });

        return await Task.FromResult((tokenHandler.WriteToken(token), expiry));
    }
}