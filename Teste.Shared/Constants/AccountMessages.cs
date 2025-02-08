namespace Teste.Shared.Constants;

public static class AccountMessages
{
    public const string PASSWORD_INCORRECT = "The provided password is incorrect.";
    public const string IDENTITY_ALREADY_REGISTERED = "The identity is already registered.";
    
    public const string EMAIL_NOT_FOUND = "No account was found with the provided email address.";
    public const string EMAIL_ALREADY_REGISTERED = "The provided email address is already registered.";

    public const string ACCOUNT_CREATED = "Account created successfully.";
    
    public const string ACCOUNT_LOCKED = "The account has been locked due to multiple failed login attempts."; // Example of adding more messages
    public const string ACCOUNT_DISABLED = "The account has been disabled. Please contact support."; // Additional example
}