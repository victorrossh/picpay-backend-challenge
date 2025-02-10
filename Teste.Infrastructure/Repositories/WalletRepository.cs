using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Teste.Domain.Entities;
using Teste.Domain.Repositories;
using Teste.Infrastructure.Contexts;
using Teste.Shared.Utilities;

namespace Teste.Infrastructure.Repositories;

public class WalletRepository(TesteDbContext context, IConfiguration configuration) : IWalletRepository
{
    private readonly string _connection = configuration.GetConfiguration<string>("Connections:SqlServer");

    public async Task<bool> AddAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
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
                .AsNoTracking()
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

    public async Task<int> TransferAsync(string payer_id, string payee_id, decimal amount,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@payer_id", Guid.Parse(payer_id), DbType.Guid);
            parameters.Add("@payee_id", Guid.Parse(payee_id), DbType.Guid);
            parameters.Add("@amount", amount, DbType.Decimal);
            parameters.Add("@transaction_status", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("sp_transfer_funds", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@transaction_status");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while fetching wallet for Id: {WalletId}", payer_id);
            throw;
        }
    }

    public async Task<Wallet?> GetByWalletIdAsync(string walletId, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == Guid.Parse(walletId), cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while fetching wallet for Id: {WalletId}", walletId);
            throw;
        }
    }

    public async Task<bool> ExistsByWalletIdAsync(string walletId, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Wallets
                .AnyAsync(w => w.Id == Guid.Parse(walletId), cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while checking existence of wallet for Id: {WalletId}", walletId);
            throw;
        }
    }
}