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
    private readonly string? _requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "Unknown";

    public async Task<TokenOut> ExecuteSignInAsync(SignInAccountIn request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validation = await new SignInValidator().ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new BadRequestException(validation.Errors.Select(er => er.ErrorMessage).ToList());

            var account = await repository.GetByEmailAsync(request.Email, cancellationToken);
            if (account is null) throw new NotFoundException([AccountMessages.EMAIL_NOT_FOUND]);

            if (!cryptography.VerifyPassword(request.Password, account.Password))
                throw new BadRequestException([AccountMessages.PASSWORD_INCORRECT]);

            var (token, expiry) = await tokenization.GenerateTokenAsync(account.Id, account.Role);

            return new TokenOut(token, _requestId, expiry);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error retrieving RequestId: {RequestId}", _requestId);
            throw new UnknownException(ex, [UnknownMessages.UNEXPECTED_ERROR]);
        }
    }
}