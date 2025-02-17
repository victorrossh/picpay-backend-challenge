namespace Teste.Application.Services.Implementations;

public interface ITokenizationImp
{
    Task<(string, string)> GenerateTokenAsync(Guid accountId);
}