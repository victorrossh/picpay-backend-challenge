using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Shared;

namespace Teste.Application.UseCases.Implementations;

public interface ISignUpImp
{
    Task<Result<DefaultRes, Error>> ExecuteSignUpAsync(SignUpReq request, CancellationToken cancellationToken);
}