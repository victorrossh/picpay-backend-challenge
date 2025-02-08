namespace Teste.Shared.Exceptions;

public class UnknownException(Exception exception, IEnumerable<string>? messages = null) : Exception
{
    public IReadOnlyList<string> Messages { get; } = new List<string>(messages ?? Array.Empty<string>());
}