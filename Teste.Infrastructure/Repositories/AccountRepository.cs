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

public class AccountRepository(TesteDbContext context, IConfiguration configuration) : IAccountRepository
{
    private readonly string _connection = configuration.GetConfiguration<string>("Connections:SqlServer");

    public async Task<bool> AddAsync(Account? account, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync(cancellationToken);

        try
        {
            var result = await connection.ExecuteAsync(
                "sp_create_account",
                new
                {
                    name = account?.Name,
                    identity = account?.Identity,
                    email = account?.Email,
                    password = account?.Password,
                    role = account?.Role
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while adding account with email: {Email}", account?.Email);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Account? account, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.Accounts.Update(account);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            Log.Error(ex, "Error occurred while updating account with Id: {AccountId}", account?.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var account = await context.Accounts.FindAsync([id], cancellationToken);
            if (account == null) return false;

            context.Accounts.Remove(account);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            Log.Error(ex, "Error occurred while deleting account with Id: {AccountId}", id);
            throw;
        }
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while fetching account with Id: {AccountId}", id);
            throw;
        }
    }

    public async Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while fetching account with email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Account?>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await context.Accounts.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while fetching all accounts");
            throw;
        }
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Accounts
                .AsNoTracking()
                .AnyAsync(a => a.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while checking if account exists with email: {Email}", email);
            throw;
        }
    }

    public async Task<bool> ExistsByIdentityAsync(string identity, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Accounts
                .AsNoTracking()
                .AnyAsync(a => a.Identity == identity, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while checking if account exists with identity: {Identity}", identity);
            throw;
        }
    }
}