using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Repositories;
using Teste.Infrastructure.Contexts;
using Teste.Shared.Utilities;

namespace Teste.Infrastructure.Repositories;

public class AccountRepository(TesteDbContext context, IConfiguration configuration) : IAccountRepository
{
    private readonly string _connection = configuration.GetConfiguration<string>("Connections:SqlServer");

    public async Task<bool> AddAsync(Account account, Role role, CancellationToken cancellationToken)
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
    }

    public async Task<bool> UpdateAsync(Account account, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.Accounts.Update(account);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var account = await context.Accounts
            .FindAsync([id], cancellationToken);
        if (account == null) return false;

        context.Accounts.Remove(account);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking().FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Account?>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking().AnyAsync(a => a.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByIdentityAsync(string identity, CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking().AnyAsync(a => a.Identity == identity, cancellationToken);
    }
}