using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Teste.Domain.Entities;
using Teste.Domain.Repositories;
using Teste.Infrastructure.Contexts;
using Teste.Shared.Utilities;

namespace Teste.Infrastructure.Repositories;

public class WalletRepository(TesteDbContext context, IConfiguration configuration) : IWalletRepository
{
    private readonly string _connection = configuration.GetConfiguration<string>("Connections:SqlServer");

    public async Task<(int?, Guid?)> TransferAsync(string payerId, string payeeId, decimal amount,
        CancellationToken cancellationToken)
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
    }

    public async Task<Wallet?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken)
    {
        return await context.Wallets
            .AsNoTracking().FirstOrDefaultAsync(w => w.AccountId == Guid.Parse(accountId), cancellationToken);
    }

    public async Task<Wallet?> GetByWalletIdAsync(string walletId, CancellationToken cancellationToken)
    {
        return await context.Wallets
            .AsNoTracking().FirstOrDefaultAsync(w => w.Id == Guid.Parse(walletId), cancellationToken);
    }

    public async Task<bool> ExistsByWalletIdAsync(string walletId, CancellationToken cancellationToken)
    {
        return await context.Wallets
            .AsNoTracking().AnyAsync(w => w.Id == Guid.Parse(walletId), cancellationToken);
    }
}