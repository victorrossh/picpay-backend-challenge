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
    public async Task<DefaultOut?> ExecuteSignUpAsync(SignUpAccountIn request, CancellationToken cancellationToken)
    {
        var requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "Unknown";

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validation = await new SignUpValidator().ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(er => er.ErrorMessage).ToList();
                Log.Warning("Sign-up validation failed. Errors: {Errors}, RequestId: {RequestId}", errors, requestId);
                throw new DetailedException(errors);
            }

            if (request.Password != request.PasswordConfirmation)
            {
                Log.Warning("Password and confirmation do not match for email: {Email}, RequestId: {RequestId}",
                    request.Email, requestId);
                throw new DetailedException([Messages.PASSWORD_MISMATCH]);
            }

            if (await accountRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                Log.Warning("Email already registered: {Email}, RequestId: {RequestId}", request.Email, requestId);
                throw new DetailedException([Messages.EMAIL_ALREADY_REGISTERED]);
            }

            if (await accountRepository.ExistsByIdentityAsync(request.Identity, cancellationToken))
            {
                Log.Warning("Identity already registered: {Identity}, RequestId: {RequestId}", request.Identity,
                    requestId);
                throw new DetailedException([Messages.IDENTITY_ALREADY_REGISTERED]);
            }

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

                Log.Information("User signed up successfully: {RequestEmail}, RequestId: {RequestId}", request.Email,
                    requestId);
                return new DefaultOut(account.Id, requestId, Messages.ACCOUNT_CREATED);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during sign-up process for email: {Email}, RequestId: {RequestId}", request.Email,
                requestId);
            throw;
        }
    }
}