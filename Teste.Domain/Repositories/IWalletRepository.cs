using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Teste.Domain.Entities;

namespace Teste.Domain.Repositories;

public interface IWalletRepository
{
    Task<bool> CreateAsync(Wallet wallet, CancellationToken cancellationToken, IDbContextTransaction transaction);
}