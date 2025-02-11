using Teste.Domain.Entities;

namespace Teste.Domain.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByAccountIdAsync(string? accountId, CancellationToken cancellationToken);
    Task<bool> ExistsByAccountIdAsync(string accountId, CancellationToken cancellationToken);
    Task<(int, Guid)> TransferAsync(string payer_id, string payee_id, decimal amount,
        CancellationToken cancellationToken);
}