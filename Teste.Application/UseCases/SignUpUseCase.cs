using Microsoft.AspNetCore.Http;
using Serilog;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.Services.Implementations;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Repositories;
using Teste.Shared.Constants;
using Teste.Shared.Exceptions;

namespace Teste.Application.UseCases;

public class SignUpUseCase(
    IAccountRepository accountRepository,
    IWalletRepository walletRepository,
    ICryptographyImp cryptography,
    IHttpContextAccessor httpContextAccessor)
    : ISignUpImp
{
    private readonly string? _requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "Unknown";

    public async Task<DefaultOut?> ExecuteSignUpAsync(SignUpAccountIn request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validation = await new SignUpValidator().ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new BadRequestException(validation.Errors.Select(er => er.ErrorMessage).ToList());

            if (await accountRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                throw new BadRequestException([AccountMessages.EMAIL_ALREADY_REGISTERED]);

            if (await accountRepository.ExistsByIdentityAsync(request.Identity, cancellationToken))
                throw new BadRequestException([AccountMessages.IDENTITY_ALREADY_REGISTERED]);

            var account = new Account
            {
                Name = request.FullName,
                Identity = request.Identity,
                Email = request.Email,
                Password = cryptography.EncryptPassword(request.Password),
                Role = request.Role
            };

            await using var transaction = await accountRepository.BeginTransactionAsync(cancellationToken);
            try
            {
                await accountRepository.AddAsync(account, cancellationToken, transaction);
                await walletRepository.CreateAsync(
                    new Wallet { AccountId = account.Id, Balance = request.Role == Role.User ? 10.0m : 0m },
                    cancellationToken, transaction);
                await transaction.CommitAsync(cancellationToken);

                return new DefaultOut(account.Id, _requestId, AccountMessages.ACCOUNT_CREATED);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
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