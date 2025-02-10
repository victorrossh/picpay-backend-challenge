namespace Teste.Shared.Exceptions;

public class BadRequestException(string[]? messages = null!)
    : Exception(string.Join(", ", messages ?? []))
{
    public IReadOnlyList<string> Messages { get; } = messages ?? [];
}