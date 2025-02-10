using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;

namespace Teste.Application.UseCases.Implementations;

public interface ISignInImp
{
    Task<TokenRes> ExecuteSignInAsync(SignInReq request, CancellationToken cancellationToken);
}