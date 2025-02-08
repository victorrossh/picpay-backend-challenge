using Teste.Application.DTOs.Responses;

namespace Teste.Application.UseCases.Implementations;

public interface IWalletImp
{
    Task<BalanceOut> GetBalanceAsync(string accountId, CancellationToken cancellationToken);
}