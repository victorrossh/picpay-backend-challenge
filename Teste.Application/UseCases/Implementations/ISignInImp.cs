using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;

namespace Teste.Application.UseCases.Implementations;

public interface ISignInImp
{
    Task<TokenOut> ExecuteSignInAsync(SignInAccountIn request, CancellationToken cancellationToken);
}