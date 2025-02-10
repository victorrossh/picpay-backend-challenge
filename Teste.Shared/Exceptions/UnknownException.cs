namespace Teste.Shared.Exceptions;

public class UnknownException(string[]? messages = null!)
    : Exception(string.Join(", ", messages ?? []))
{
    public IReadOnlyList<string> Messages { get; } = messages ?? [];
}