using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases.Validators;

public class SignUpValidator : AbstractValidator<SignUpReq>
{
    public SignUpValidator()
    {
        RuleFor(e => e.email)
            .NotEmpty()
            .WithMessage(AccountMessages.EMAIL_REQUIRED)
            .EmailAddress()
            .WithMessage(AccountMessages.EMAIL_INVALID);

        RuleFor(c => c.password)
            .NotEmpty()
            .WithMessage(AccountMessages.PASSWORD_REQUIRED)
            .Length(8, 16)
            .WithMessage(AccountMessages.PASSWORD_INVALID)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$")
            .WithMessage(AccountMessages.PASSWORD_INVALID);

        RuleFor(c => c.identity)
            .NotEmpty()
            .WithMessage(AccountMessages.IDENTITY_REQUIRED)
            .Matches(@"^\d{11}$|^\d{14}$")
            .WithMessage(AccountMessages.IDENTITY_INVALID)
            .Matches(@"^\d+$")
            .WithMessage(AccountMessages.IDENTITY_INVALID);

        RuleFor(c => c.name)
            .NotEmpty()
            .WithMessage(AccountMessages.NAME_REQUIRED)
            .Matches(@"^[A-Za-zÀ-ÖØ-öø-ÿ\s]{3,100}$")
            .WithMessage(AccountMessages.NAME_INVALID);
    }
}