namespace Teste.Shared.Exceptions;

public class BadRequestException(IEnumerable<string>? messages = null) : Exception
{
    public IReadOnlyList<string> Messages { get; } = new List<string>(messages ?? Array.Empty<string>());
}