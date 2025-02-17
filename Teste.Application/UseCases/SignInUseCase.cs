using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.Services.Implementations;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Repositories;
using Teste.Shared;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases;

public class SignInUseCase(
    IAccountRepository repository,
    ITokenizationImp tokenization,
    ICryptographyImp cryptography)
    : ISignInImp
{
    public async Task<Result<TokenRes, Error>> ExecuteSignInAsync(SignInReq request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validation = await new SignInValidator().ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<TokenRes, Error>.Failure(
                new ValidationError(validation.Errors.Select(er => er.ErrorMessage).ToList()))!;

        var account = await repository.GetByEmailAsync(request.email, cancellationToken);
        if (account is null)
            return Result<TokenRes, Error>.Failure(new NotFoundError([AccountMessages.EMAIL_NOT_FOUND]))!;

        if (!cryptography.VerifyPassword(request.password, account.Password))
            return Result<TokenRes, Error>.Failure(new BadRequestError([AccountMessages.PASSWORD_INCORRECT]))!;

        var (token, expiry) = await tokenization.GenerateTokenAsync(account.Id);

        return Result<TokenRes, Error>.Success(new TokenRes(token, expiry))!;
    }
}