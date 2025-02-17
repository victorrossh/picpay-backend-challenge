namespace Teste.Shared.Constants;

public static class AccountMessages
{
    public const string IDENTITY_ALREADY_REGISTERED = "The identity is already registered. Please check and try again.";

    public const string EMAIL_NOT_FOUND =
        "The account with the provided email address was not found. Please check and try again.";

    public const string EMAIL_ALREADY_REGISTERED =
        "The provided email address is already registered. Please check and try again.";

    // Email-related messages
    public const string EMAIL_INVALID = "The provided email address is invalid. Please check and try again.";
    public const string EMAIL_REQUIRED = "The email address is required for the account. Please check and try again.";

    // Password-related messages
    public const string PASSWORD_INCORRECT = "The provided password is incorrect. Please check and try again.";
    public const string PASSWORD_INVALID = "The provided password is invalid. Please check and try again.";
    public const string PASSWORD_REQUIRED = "The password is required for the account. Please check and try again.";
    public const string PASSWORD_MISMATCH = "The provided passwords do not match. Please check and try again.";

    // Identity-related messages
    public const string IDENTITY_INVALID = "The provided identity is invalid. Please check and try again.";
    public const string IDENTITY_REQUIRED = "The identity is required for the account. Please check and try again.";

    // Role-related messages
    public const string ROLE_INVALID = "The provided role is invalid. Please check and try again.";
    public const string ROLE_REQUIRED = "The role is required for the account. Please check and try again.";

    // Name-related messages
    public const string NAME_REQUIRED = "The name of the account holder is required. Please check and try again.";
    public const string NAME_INVALID = "The provided name is invalid. Please check and try again.";

    // Account creation response
    public const string ACCOUNT_CREATED = "The account was created successfully.";
}