using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Shared;

namespace Teste.Application.UseCases.Implementations;

public interface IWalletImp
{
    Task<Result<BalanceRes, Error>> BalanceAsync(CancellationToken cancellationToken);
    Task<Result<TransferRes, Error>> TransferAsync(TransferReq request, CancellationToken cancellationToken);
}