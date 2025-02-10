using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;

namespace Teste.Application.UseCases.Implementations;

public interface ISignUpImp
{
    Task<DefaultRes?> ExecuteSignUpAsync(SignUpReq request, CancellationToken cancellationToken);
}