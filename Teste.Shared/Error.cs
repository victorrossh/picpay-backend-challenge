namespace Teste.Shared;

public abstract class Error(IEnumerable<string> messages)
{
    public IReadOnlyList<string> Messages { get; } = messages.ToList();
}

public class ValidationError(IEnumerable<string> messages) : Error(messages);

public class NotFoundError(IEnumerable<string> messages) : Error(messages);

public class BadRequestError(IEnumerable<string> messages) : Error(messages);