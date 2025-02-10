using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases.Validators;

public class TransferValidator : AbstractValidator<TransferReq>
{
    public TransferValidator()
    {
        RuleFor(x => x.payeeId)
            .NotEmpty()
            .WithMessage("Payee ID is required.")
            .Matches("^[a-zA-Z0-9-]+$")
            .WithMessage("Payee ID is not valid.");

        RuleFor(x => x.amount)
            .GreaterThan(0)
            .WithMessage(WalletMessages.AMOUNT_INVALID)
            .NotNull()
            .WithMessage(WalletMessages.AMOUNT_INVALID);
    }
}