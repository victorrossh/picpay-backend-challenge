using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Repositories;
using Teste.Infrastructure.Contexts;
using Teste.Shared.Utilities;

namespace Teste.Infrastructure.Repositories;

public class AccountRepository(TesteDbContext context, IConfiguration configuration) : IAccountRepository
{
    private readonly string _connection = configuration.GetConfiguration<string>("Connections:SqlServer");

    public async Task<bool> AddAsync(Account? account, Role role, CancellationToken cancellationToken)
    {
        if (account is null) return false;

        return await TryExecuteAsync(async () =>
        {
            await using var connection = new SqlConnection(_connection);
            await connection.OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("@name", account.Name, DbType.String);
            parameters.Add("@identity", account.Identity, DbType.String);
            parameters.Add("@email", account.Email, DbType.String);
            parameters.Add("@password", account.Password, DbType.String);
            parameters.Add("@role", role, DbType.Int32);

            var result = await connection.ExecuteAsync(
                "sp_create_account",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }, "adding account", account.Email);
    }


    public async Task<bool> UpdateAsync(Account? account, CancellationToken cancellationToken)
    {
        if (account is null) return false;

        return await TryExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            context.Accounts.Update(account);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }, "updating account", account.Id);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            var account = await context.Accounts.FindAsync([id], cancellationToken);
            if (account == null) return false;

            context.Accounts.Remove(account);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }, "deleting account", id);
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
                await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken),
            "fetching account by Id", id);
    }

    public async Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
                await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Email == email, cancellationToken),
            "fetching account by Email", email);
    }

    public async Task<IEnumerable<Account?>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
                await context.Accounts.AsNoTracking().ToListAsync(cancellationToken),
            "fetching all accounts") ?? Enumerable.Empty<Account>();
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
                await context.Accounts.AsNoTracking().AnyAsync(a => a.Email == email, cancellationToken),
            "checking account existence by Email", email);
    }

    public async Task<bool> ExistsByIdentityAsync(string identity, CancellationToken cancellationToken)
    {
        return await TryExecuteAsync(async () =>
                await context.Accounts.AsNoTracking().AnyAsync(a => a.Identity == identity, cancellationToken),
            "checking account existence by Identity", identity);
    }

    private static async Task<T?> TryExecuteAsync<T>(Func<Task<T>> action, string operation, object? id = null)
    {
        try
        {
            return await action();
        }
        catch (IOException ex)
        {
            Log.Error(ex, "Error occurred while {Operation} for Id: {Id}", operation, id);
            throw;
        }
    }
}