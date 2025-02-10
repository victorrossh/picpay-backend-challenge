using Teste.Domain.Entities;

namespace Teste.Domain.Repositories;

public interface IWalletRepository
{
    Task<bool> AddAsync(Wallet wallet, CancellationToken cancellationToken);
    Task<Wallet?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken);
    Task<bool> ExistsByAccountIdAsync(string accountId, CancellationToken cancellationToken);
    Task<int> TransferAsync(string payer_id, string payee_id, decimal amount, CancellationToken cancellationToken);
}