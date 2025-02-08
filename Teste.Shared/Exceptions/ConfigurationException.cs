namespace Teste.Shared.Exceptions;

public class ConfigurationException(IEnumerable<string>? messages = null) : Exception
{
    public IReadOnlyList<string> Messages { get; } = new List<string>(messages ?? Array.Empty<string>());
}