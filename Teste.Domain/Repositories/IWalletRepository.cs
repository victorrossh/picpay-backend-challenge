using Microsoft.EntityFrameworkCore.Storage;
using Teste.Domain.Entities;

namespace Teste.Domain.Repositories;

public interface IWalletRepository
{
    Task<bool> CreateAsync(Wallet wallet, CancellationToken cancellationToken, IDbContextTransaction transaction);
    Task<Wallet?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken);
    Task<bool> ExistsByAccountIdAsync(string accountId, CancellationToken cancellationToken);
}