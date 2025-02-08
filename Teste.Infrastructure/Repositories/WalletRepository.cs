using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Teste.Domain.Entities;
using Teste.Domain.Repositories;
using Teste.Infrastructure.Contexts;

namespace Teste.Infrastructure.Repositories;

public class WalletRepository(TesteDbContext context) : IWalletRepository
{
    public async Task<bool> CreateAsync(Wallet wallet, CancellationToken cancellationToken,
        IDbContextTransaction transaction)
    {
        try
        {
            await context.Wallets.AddAsync(wallet, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            Log.Error(ex, "Error occurred while creating wallet for AccountId: {AccountId}", wallet.AccountId);
            return false;
        }
    }

    public async Task<Wallet?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Wallets
                .FirstOrDefaultAsync(w => w.AccountId == Guid.Parse(accountId), cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while fetching wallet for AccountId: {AccountId}", accountId);
            throw;
        }
    }

    public async Task<bool> ExistsByAccountIdAsync(string accountId, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Wallets
                .AnyAsync(w => w.AccountId == Guid.Parse(accountId), cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while checking existence of wallet for AccountId: {AccountId}", accountId);
            throw;
        }
    }
}