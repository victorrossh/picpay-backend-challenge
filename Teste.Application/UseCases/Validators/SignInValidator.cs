using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases.Validators;

public class SignInValidator : AbstractValidator<SignInReq>
{
    public SignInValidator()
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
    }
}