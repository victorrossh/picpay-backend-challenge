using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Teste.Domain.Entities;
using Teste.Domain.Repositories;
using Teste.Infrastructure.Contexts;

public class AccountRepository(TesteDbContext context) : IAccountRepository
{
    public async Task<bool> AddAsync(Account account, CancellationToken cancellationToken,
        IDbContextTransaction transaction)
    {
        try
        {
            await context.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

            await context.Accounts.AddAsync(account, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while adding account with Id: {AccountId}", account.Id);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Account account, CancellationToken cancellationToken,
        IDbContextTransaction transaction)
    {
        try
        {
            await context.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

            context.Accounts.Update(account);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while updating account with Id: {AccountId}", account.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken, IDbContextTransaction transaction)
    {
        try
        {
            await context.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

            var account = await context.Accounts.FindAsync([id], cancellationToken);
            if (account == null) return false;

            context.Accounts.Remove(account);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while deleting account with Id: {AccountId}", id);
            throw;
        }
    }

    // Métodos de consulta não precisam de transação, por isso são simples
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Accounts
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
                .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while fetching account with email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await context.Accounts.ToListAsync(cancellationToken);
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
                .AnyAsync(a => a.Identity == identity, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while checking if account exists with identity: {Identity}", identity);
            throw;
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }
}