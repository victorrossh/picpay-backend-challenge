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
            await context.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
            await context.Wallets.AddAsync(wallet, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while creating wallet for AccountId: {AccountId}", wallet.AccountId);
            throw;
        }
    }
}