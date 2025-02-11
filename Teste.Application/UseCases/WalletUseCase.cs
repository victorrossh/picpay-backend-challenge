using Microsoft.AspNetCore.Http;
using Serilog;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Enums;
using Teste.Domain.Repositories;
using Teste.Shared.Constants;
using Teste.Shared.Exceptions;

namespace Teste.Application.UseCases;

public class WalletUseCase(
    IWalletRepository repository,
    IHttpContextAccessor httpContextAccessor) : IWalletImp
{
    private readonly string? _requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "Unknown";

    public async Task<BalanceRes> BalanceAsync(string? accountId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var wallet = await repository.GetByAccountIdAsync(accountId, cancellationToken);
            if (wallet is null)
                throw new NotFoundException([WalletMessages.WALLET_NOT_FOUND]);

            return new BalanceRes(wallet.Id, wallet.Balance);
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
            throw new UnknownException([UnknownMessages.UNEXPECTED_ERROR]);
        }
    }

    public async Task<TransferRes> TransferAsync(string? accountId, TransferReq request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var validation = await new TransferValidator().ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new BadRequestException(validation.Errors.Select(er => er.ErrorMessage).ToArray());

            var wallet = await repository.GetByAccountIdAsync(accountId, cancellationToken);
            if (string.Equals(wallet!.Id.ToString(), request.payeeId, StringComparison.CurrentCultureIgnoreCase))
                throw new BadRequestException([WalletMessages.ACCOUNT_CANNOT_TRANSFER]);

            if (wallet.Role == Role.Pj)
            {
                throw new BadRequestException([WalletMessages.ACCOUNT_PJ_CANNOT_TRANSFER]);
            }

            var (transferStatus, transferId) =
                await repository.TransferAsync(wallet.Id.ToString(), request.payeeId, request.amount,
                    cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                throw new BadRequestException([WalletMessages.TRANSACTION_CANCELLED]);

            return transferStatus == 1
                ? new TransferRes(transferId, WalletMessages.TRANSFER_SUCCESSFUL)
                : throw new BadRequestException([WalletMessages.TRANSFER_FAILED]);
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
            Log.Error(ex, "Unexpected error while processing transfer for RequestId: {RequestId}", _requestId);
            throw new UnknownException([UnknownMessages.UNEXPECTED_ERROR]);
        }
    }
}