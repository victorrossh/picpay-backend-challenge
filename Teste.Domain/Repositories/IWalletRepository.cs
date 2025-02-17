using Teste.Domain.Entities;

namespace Teste.Domain.Repositories;

public interface IWalletRepository
{
    Task<(int?, Guid?)> TransferAsync(string payerId, string payeeId, decimal amount,
        CancellationToken cancellationToken);

    Task<Wallet?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken);
    Task<Wallet?> GetByWalletIdAsync(string walletId, CancellationToken cancellationToken);
    Task<bool> ExistsByWalletIdAsync(string walletId, CancellationToken cancellationToken);
}