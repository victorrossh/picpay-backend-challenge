using Microsoft.AspNetCore.Http;
using Serilog;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;
using Teste.Domain.Repositories;
using Teste.Shared.Constants;
using Teste.Shared.Exceptions;

namespace Teste.Application.UseCases;

public class WalletUseCase(
    IWalletRepository repository,
    IHttpContextAccessor httpContextAccessor) : IWalletImp
{
    private readonly string? _requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "Unknown";

    public async Task<BalanceOut> GetBalanceAsync(string accountId, CancellationToken cancellationToken)
    {
        try
        {
            var wallet = await repository.GetByAccountIdAsync(accountId, cancellationToken);
            if (wallet is null)
                throw new NotFoundException([WalletMessages.WALLET_NOT_FOUND]);

            return new BalanceOut(wallet.Id, wallet.Balance);
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