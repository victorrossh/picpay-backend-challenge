using Microsoft.AspNetCore.Http;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Enums;
using Teste.Domain.Repositories;
using Teste.Shared;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases;

public class WalletUseCase(
    IWalletRepository repository,
    IHttpContextAccessor httpContextAccessor) : IWalletImp
{
    public async Task<Result<TransferRes, Error>> TransferAsync(TransferReq request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var accountId = httpContextAccessor.HttpContext.Items["AccountId"]!.ToString()!;

        var validation = await new TransferValidator().ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<TransferRes, Error>.Failure(
                new ValidationError(validation.Errors.Select(er => er.ErrorMessage).ToList()))!;

        var wallet = await repository.GetByAccountIdAsync(accountId, cancellationToken);
        if (string.Equals(wallet!.Id.ToString(), request.payeeId, StringComparison.CurrentCultureIgnoreCase))
            return Result<TransferRes, Error>.Failure(new BadRequestError([
                WalletMessages.ACCOUNT_CANNOT_TRANSFER
            ]))!;

        if (wallet.Role == Role.Pj)
            return Result<TransferRes, Error>.Failure(new BadRequestError([
                WalletMessages.ACCOUNT_PJ_CANNOT_TRANSFER
            ]))!;

        var (transferStatus, transferId) =
            await repository.TransferAsync(wallet.Id.ToString(), request.payeeId, request.amount,
                cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return Result<TransferRes, Error>.Failure(new BadRequestError([WalletMessages.TRANSACTION_CANCELLED]))!;

        return transferStatus == 1
            ? Result<TransferRes, Error>.Success(new TransferRes(transferId, WalletMessages.TRANSFER_SUCCESSFUL))!
            : Result<TransferRes, Error>.Failure(new BadRequestError([WalletMessages.TRANSFER_FAILED]))!;
    }

    public async Task<Result<BalanceRes, Error>> BalanceAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var accountId = httpContextAccessor.HttpContext.Items["AccountId"]!.ToString()!;

        var wallet = await repository.GetByAccountIdAsync(accountId, cancellationToken);
        if (wallet is null)
            return Result<BalanceRes, Error>.Failure(new NotFoundError([
                WalletMessages.WALLET_NOT_FOUND
            ]))!;

        return Result<BalanceRes, Error>.Success(new BalanceRes(wallet.Id, wallet.Balance))!;
    }
}