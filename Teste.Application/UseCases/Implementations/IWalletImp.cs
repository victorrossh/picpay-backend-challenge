using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;

namespace Teste.Application.UseCases.Implementations;

public interface IWalletImp
{
    Task<BalanceRes> BalanceAsync(string accountId, CancellationToken cancellationToken);
    Task<DefaultRes> TransferAsync(string accountId, TransferReq request, CancellationToken cancellationToken);
}