using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Shared;

namespace Teste.Application.UseCases.Implementations;

public interface ISignInImp
{
    Task<Result<TokenRes, Error>> ExecuteSignInAsync(SignInReq request, CancellationToken cancellationToken);
}