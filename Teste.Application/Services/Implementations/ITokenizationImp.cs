using Teste.Domain.Enums;

namespace Teste.Application.Services.Implementations;

public interface ITokenizationImp
{
    Task<(string, DateTime)> GenerateTokenAsync(Guid accountId);
}