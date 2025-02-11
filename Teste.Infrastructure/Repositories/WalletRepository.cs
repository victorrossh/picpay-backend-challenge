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

    public async Task<(int, Guid)> TransferAsync(string payerId, string payeeId, decimal amount,
        CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
        {
            await using var connection = new SqlConnection(_connection);
            await connection.OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("@payer_id", Guid.Parse(payerId), DbType.Guid);
            parameters.Add("@payee_id", Guid.Parse(payeeId), DbType.Guid);
            parameters.Add("@amount", amount, DbType.Decimal);
            parameters.Add("@transaction_status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@transaction_id", dbType: DbType.Guid, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("sp_transfer_funds", parameters, commandType: CommandType.StoredProcedure,
                commandTimeout: 120);

            return (parameters.Get<int>("@transaction_status"), parameters.Get<Guid>("@transaction_id"));
        }, null, "transferring funds", payerId);
    }

    public async Task<Wallet?> GetByAccountIdAsync(string? accountId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            Log.Warning("AccountId is null or empty while attempting to fetch wallet");
            return null;
        }

        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            Log.Warning("Invalid AccountId format: {AccountId}. Could not parse as GUID", accountId);
            return null;
        }

        return await TryExecuteAsync(async () =>
                await context.Wallets.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.AccountId == accountGuid, cancellationToken),
            null, "fetching wallet", accountId);
    }

    public Task<bool> ExistsByAccountIdAsync(string accountId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Wallet?> GetByWalletIdAsync(string walletId, CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
                await context.Wallets.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.Id == Guid.Parse(walletId), cancellationToken),
            null, "fetching wallet by Id", walletId);
    }

    public async Task<bool> ExistsByWalletIdAsync(string walletId, CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
                await context.Wallets.AnyAsync(w => w.Id == Guid.Parse(walletId), cancellationToken),
            null, "checking existence of wallet by Id", walletId);
    }

    public async Task<(Wallet? PayerWallet, Wallet? PayeeWallet)> GetWalletsForTransferAsync(string payerId,
        string payeeId, CancellationToken cancellationToken)
    {
        // Run two queries in parallel (independently) using Task.WhenAll
        var payerWalletTask = GetByAccountIdAsync(payerId, cancellationToken);
        var payeeWalletTask = GetByAccountIdAsync(payeeId, cancellationToken);

        // Wait for both tasks to complete concurrently
        await Task.WhenAll(payerWalletTask, payeeWalletTask);

        return (payerWalletTask.Result, payeeWalletTask.Result);
    }

    // Update the TryExecuteAsync method to handle parallelism with cancellation support and retries
    private static async Task<T> TryExecuteAsync<T>(Func<Task<T>> action, Func<Task>? rollback, string operation,
        object? id)
    {
        var maxRetries = 3; // Maximum number of retries for deadlocks
        var retryDelay = 2000; // Delay between retries in milliseconds

        for (var attempt = 0; attempt < maxRetries; attempt++)
            try
            {
                return await action();
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 1205) // Deadlock error code is 1205
            {
                Log.Warning(sqlEx,
                    "Deadlock detected while {Operation} for Id: {Id}. Retrying... Attempt {Attempt}/{MaxRetries}",
                    operation, id, attempt + 1, maxRetries);

                if (attempt < maxRetries - 1)
                {
                    await Task.Delay(retryDelay);
                }
                else
                {
                    Log.Error(sqlEx, "Deadlock occurred while {Operation} for Id: {Id} and retries were exhausted",
                        operation, id);
                    if (rollback is not null) await rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while {Operation} for Id: {Id}", operation, id);
                if (rollback is not null) await rollback();
                throw;
            }

        throw new InvalidOperationException($"Failed to execute {operation} after {maxRetries} attempts for Id: {id}");
    }
}