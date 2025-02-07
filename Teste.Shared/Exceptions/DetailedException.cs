namespace Teste.Shared.Exceptions;

/// <summary>
///     Represents a detailed exception with an optional list of error messages for additional context.
/// </summary>
public class DetailedException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DetailedException" /> class with a default message.
    /// </summary>
    /// <param name="errorMessages">A list of error messages providing additional details (optional).</param>
    public DetailedException(IEnumerable<string>? errorMessages = null)
    {
        ErrorMessages = new List<string>(errorMessages ?? Array.Empty<string>());
    }

    /// <summary>
    ///     Gets the list of error messages associated with the exception.
    /// </summary>
    public IReadOnlyList<string> ErrorMessages { get; }

    /// <summary>
    ///     Returns a string representation of the exception, including error messages if available.
    /// </summary>
    /// <returns>A string that represents the current exception.</returns>
    public override string ToString()
    {
        var baseMessage = base.ToString();
        return ErrorMessages.Count > 0
            ? $"{baseMessage}{Environment.NewLine}Error Producers: {string.Join(", ", ErrorMessages)}"
            : baseMessage;
    }
}