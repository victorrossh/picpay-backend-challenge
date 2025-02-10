namespace Teste.Shared.Constants;

public static class WalletMessages
{
    // Error messages
    public const string WALLET_NOT_FOUND =
        "The wallet for the provided account was not found. Please ensure the account exists.";

    public const string WALLET_CREATION_FAILED = "The wallet creation failed. Please check and try again.";
    public const string WALLET_CREATED_SUCCESSFULLY = "The wallet was created successfully.";

    public const string TRANSFER_SUCCESSFUL = "The transfer was successful.";

    public const string TRANSFER_FAILED = "The transfer failed. Please check and try again.";
    public const string TRANSACTION_CANCELLED = "The transfer cancelled. Please check and try again.";

    // Exception messages
    public const string ACCOUNT_ALREADY_HAS_WALLET = "The account already has an associated wallet.";
    public const string ACCOUNT_CANNOT_TRANSFER = "The account cannot transfer to itself.";

    public const string AMOUNT_INVALID = "The specified amount is invalid. Please check and try again.";
    public const string AMOUNT_REQUIRED = "The specified amount is required. Please check and try again.";

    public const string PAYEE_INVALID = "The specified payee is invalid. Please check and try again.";
    public const string PAYEE_REQUIRED = "The payee is required. Please check and try again.";

    public const string BALANCE_INSUFFICIENT =
        "The wallet has insufficient balance for this operation. Please check and try again.";
}
