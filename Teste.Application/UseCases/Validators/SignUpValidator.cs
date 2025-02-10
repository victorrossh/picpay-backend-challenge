using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Domain.Enums;
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
            .Length(11, 14)
            .WithMessage(AccountMessages.IDENTITY_INVALID)
            .Matches(@"^\d+$")
            .WithMessage(AccountMessages.IDENTITY_INVALID)
            .Must((request, identity) =>
            {
                return request.role switch
                {
                    Role.User when identity.Length == 11 => true,
                    Role.Retailer when identity.Length == 14 => true,
                    _ => false
                };
            })
            .WithMessage(AccountMessages.IDENTITY_INVALID);

        RuleFor(c => c.role)
            .IsInEnum()
            .WithMessage(AccountMessages.ROLE_INVALID);

        RuleFor(c => c.name)
            .NotEmpty()
            .WithMessage(AccountMessages.NAME_REQUIRED)
            .Matches(@"^[A-Za-zÀ-ÖØ-öø-ÿ\s]{3,100}$")
            .WithMessage(AccountMessages.NAME_INVALID);
    }
}