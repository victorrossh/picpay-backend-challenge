using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.Services.Implementations;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Repositories;
using Teste.Shared;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases;

public class SignUpUseCase(
    IAccountRepository accountRepository,
    ICryptographyImp cryptography)
    : ISignUpImp
{
    public async Task<Result<DefaultRes, Error>> ExecuteSignUpAsync(SignUpReq request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validation = await new SignUpValidator().ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<DefaultRes, Error>.Failure(
                new ValidationError(validation.Errors.Select(er => er.ErrorMessage).ToList()))!;

        if (await accountRepository.ExistsByEmailAsync(request.email, cancellationToken))
            return Result<DefaultRes, Error>.Failure(new BadRequestError([
                AccountMessages.EMAIL_ALREADY_REGISTERED
            ]))!;

        if (await accountRepository.ExistsByIdentityAsync(request.identity, cancellationToken))
            return Result<DefaultRes, Error>.Failure(
                new BadRequestError([AccountMessages.IDENTITY_ALREADY_REGISTERED]))!;

        var account = new Account
        {
            Name = request.name,
            Identity = request.identity,
            Email = request.email,
            Password = cryptography.EncryptPassword(request.password)
        };

        await accountRepository.AddAsync(account, request.identity.Length == 11 ? Role.Pf : Role.Pj,
            cancellationToken);

        return Result<DefaultRes, Error>.Success(new DefaultRes(account.Id.ToString(),
            AccountMessages.ACCOUNT_CREATED))!;
    }
}