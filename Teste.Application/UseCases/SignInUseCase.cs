using Microsoft.AspNetCore.Http;
using Serilog;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.Services.Implementations;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Repositories;
using Teste.Shared.Constants;
using Teste.Shared.Exceptions;

namespace Teste.Application.UseCases;

public class SignInUseCase(
    IAccountRepository repository,
    ITokenizationImp tokenization,
    ICryptographyImp cryptography,
    IHttpContextAccessor httpContextAccessor)
    : ISignInImp
{
    public async Task<TokenOut> ExecuteSignInAsync(SignInAccountIn request, CancellationToken cancellationToken)
    {
        var requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "Unknown";

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validation = await new SignInValidator().ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(er => er.ErrorMessage).ToList();
                Log.Warning("Sign-in validation failed for email: {Email}, RequestId: {RequestId}. Errors: {Errors}",
                    request.Email, requestId, errors);
                throw new DetailedException(errors);
            }

            var account = await repository.GetByEmailAsync(request.Email, cancellationToken);
            if (account is null)
            {
                Log.Warning("No account found for email: {Email}, RequestId: {RequestId}", request.Email, requestId);
                throw new DetailedException([Messages.EMAIL_NOT_FOUND]);
            }

            if (!cryptography.VerifyPassword(request.Password, account.Password))
            {
                Log.Warning("Invalid password attempt for email: {Email}, RequestId: {RequestId}", request.Email,
                    requestId);
                throw new DetailedException([Messages.PASSWORD_INVALID]);
            }

            var (token, expiry) = await tokenization.GenerateTokenAsync(account.Id, account.Role);

            Log.Information("User signed in successfully for email: {Email}, RequestId: {RequestId}", request.Email,
                requestId);

            return new TokenOut(token, requestId, expiry);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during sign-in process for email: {Email}, RequestId: {RequestId}", request.Email,
                requestId);
            throw;
        }
    }
}