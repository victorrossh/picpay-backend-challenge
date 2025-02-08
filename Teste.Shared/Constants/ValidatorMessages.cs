namespace Teste.Shared.Constants;

public static class ValidatorMessages
{
    // Email-related messages
    public const string EMAIL_INVALID = "The email address provided is invalid.";
    public const string EMAIL_REQUIRED = "An email address is required for the account.";

    // Password-related messages
    public const string PASSWORD_INVALID = "The password provided is invalid.";
    public const string PASSWORD_REQUIRED = "Password is required for the account.";
    public const string PASSWORD_MISMATCH = "The provided passwords do not match.";
    
    // Identity-related messages
    public const string IDENTITY_INVALID = "The provided identity is invalid.";
    public const string IDENTITY_REQUIRED = "Identity is required for the account.";
    
    // Role-related messages
    public const string ROLE_INVALID = "The provided role is invalid.";
    public const string ROLE_REQUIRED = "Role is required to assign to the account.";
    
    // Name-related messages
    public const string NAME_REQUIRED = "The full name of the account holder is required.";
    public const string NAME_INVALID = "The full name provided is invalid.";

    // Account creation response
    public const string ACCOUNT_CREATED = "Account created successfully.";
}