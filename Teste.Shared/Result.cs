namespace Teste.Shared;

public class Result<T, E> where E : Error?
{
    // Private constructor to enforce usage of Success or Failure methods
    private Result(T? value, E? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }  // Nullable value for failure cases
    public E? Error { get; }  // Nullable error for success cases

    // Determines if the result is a success
    public bool IsSuccess => Error == null;
    
    // Determines if the result is a failure
    public bool IsFailure => !IsSuccess;

    // Static method to create a successful result (non-nullable Value)
    public static Result<T, E?> Success(T value)
    {
        return new Result<T, E?>(value, null);
    }

    // Static method to create a failed result (nullable Value)
    public static Result<T?, E> Failure(E error)
    {
        return new Result<T?, E>(default, error);
    }
}