namespace Teste.Shared.Constants;

public static class Messages
{
    // Session-related messages
    public const string SESSION_ACTIVE = "Session active.";
    public const string SESSION_DESACTIVE = "Session deactivated.";
    public const string ACCOUNT_NOT_FOUND = "No account found for email";
    public const string ACCOUNT_DEACTIVATED = "Account deactivated.";

    // Email-related messages
    public const string EMAIL_INVALID = "The account's email is invalid.";

    public const string EMAIL_NOT_AUTHENTICATED =
        "The email is not authenticated, new code has been verified for your email.";

    public const string EMAIL_REQUIRED = "The account's email must be provided.";
    public const string EMAIL_NOT_FOUND = "The account's email was not found.";
    public const string EMAIL_ALREADY_REGISTERED = "The account's email provided is already registered.";

    // Password-related messages
    public const string PASSWORD_INVALID = "The account's password is invalid.";
    public const string PASSWORD_MISMATCH = "The account's password does not match.";
    public const string PASSWORD_REQUIRED = "The account's password must be entered.";
    public const string PASSWORD_MIN_LENGTH = "The account's password must contain at least 8 characters.";
    public const string PASSWORD_MAX_LENGTH = "The account's password must contain a maximum of 16 characters.";

    // General messages
    public const string INVALID_CODE = "The code is invalid.";
    public const string UNKNOWN_ERROR = "Unknown error.";


    // Code-related responses
    public const string CODE_SENT = "Sending the code to your email or phone.";
    public const string CODE_CONFIRMED = "Confirmed successfully.";

    // Password-related responses
    public const string PASSWORD_RESET = "Password reset successfully.";
    public const string IDENTITY_REQUIRED = "Identity is required.";
    public const string INVALID_ROLE = "Role is invalid.";
    public const string IDENTITY_ALREADY_REGISTERED = "The identity is already registered.";
    public const string ACCOUNT_CREATED = "Account created successfully.";
    public const string FULLNAME_REQUIRED = "The full name is required.";

    public const string FULLNAME_INVALID =
        "The full name must contain only letters and spaces, between 3 and 100 characters";

    public const string IDENTITY_INVALID = "The account's identity is invalid.";

}