using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;

namespace Teste.Application.UseCases.Implementations;

public interface ISignUpImp
{
    Task<DefaultOut?> ExecuteSignUpAsync(SignUpAccountIn request, CancellationToken cancellationToken);
}