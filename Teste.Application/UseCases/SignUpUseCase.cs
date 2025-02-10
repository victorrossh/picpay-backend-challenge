using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.Services.Implementations;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Entities;
using Teste.Domain.Repositories;
using Teste.Shared.Constants;
using Teste.Shared.Exceptions;

namespace Teste.Application.UseCases;

public class SignUpUseCase(
    IAccountRepository accountRepository,
    ICryptographyImp cryptography)
    : ISignUpImp
{
    public async Task<DefaultRes?> ExecuteSignUpAsync(SignUpReq request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validation = await new SignUpValidator().ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new BadRequestException(validation.Errors.Select(er => er.ErrorMessage).ToArray());

            if (await accountRepository.ExistsByEmailAsync(request.email, cancellationToken))
                throw new BadRequestException([AccountMessages.EMAIL_ALREADY_REGISTERED]);

            if (await accountRepository.ExistsByIdentityAsync(request.identity, cancellationToken))
                throw new BadRequestException([AccountMessages.IDENTITY_ALREADY_REGISTERED]);

            var account = new Account
            {
                Name = request.name,
                Identity = request.identity,
                Email = request.email,
                Password = cryptography.EncryptPassword(request.password),
                Role = request.role
            };

            await accountRepository.AddAsync(account, cancellationToken);

            return new DefaultRes(account.Id.ToString(), AccountMessages.ACCOUNT_CREATED);
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new UnknownException([UnknownMessages.UNEXPECTED_ERROR]);
        }
    }
}