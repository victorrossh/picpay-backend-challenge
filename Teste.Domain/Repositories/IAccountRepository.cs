using Teste.Domain.Entities;
using Teste.Domain.Enums;

namespace Teste.Domain.Repositories;

public interface IAccountRepository
{
    Task<bool> AddAsync(Account account, Role role, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Account account, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<Account?>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByIdentityAsync(string identity, CancellationToken cancellationToken);
}