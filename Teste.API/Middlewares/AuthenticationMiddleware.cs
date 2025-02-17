using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Teste.Shared.Exceptions;

namespace Teste.API.Middlewares;

public class AuthenticationMiddleware(RequestDelegate next, TokenValidationParameters tokenValidationParameters)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Items["RequestId"] = Activity.Current?.Id;

        if (context.GetEndpoint()?.Metadata.GetMetadata<AuthorizeAttribute>() == null)
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            throw new UnknownException(["Authentication token is required to access this resource."]);

        var token = authHeader["Bearer ".Length..].Trim();

        if (!TryValidateToken(token, out var accountId))
            throw new UnknownException(["Invalid or expired authentication token."]);

        context.Items["AccountId"] = accountId;

        await next(context);
    }

    private bool TryValidateToken(string token, out string accountId)
    {
        accountId = string.Empty;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, tokenValidationParameters, out _);

            accountId = principal.Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid, StringComparison.OrdinalIgnoreCase))?
                .Value ?? string.Empty;

            return !string.IsNullOrEmpty(accountId);
        }
        catch (Exception)
        {
            return false;
        }
    }
}