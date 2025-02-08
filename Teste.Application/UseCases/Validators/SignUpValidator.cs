using System.Text.RegularExpressions;
using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Domain.Enums;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases.Validators
{
    public class SignUpValidator : AbstractValidator<SignUpAccountIn>
    {
        private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$",
            RegexOptions.None, TimeSpan.FromSeconds(1));

        private static readonly Regex NameRegex = new(@"^[A-Za-zÀ-ÖØ-öø-ÿ\s]{3,100}$", RegexOptions.None,
            TimeSpan.FromSeconds(1));

        public SignUpValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .WithMessage(ValidatorMessages.EMAIL_REQUIRED)
                .EmailAddress()
                .WithMessage(ValidatorMessages.EMAIL_INVALID);

            RuleFor(c => c.Password)
                .NotEmpty()
                .WithMessage(ValidatorMessages.PASSWORD_REQUIRED)
                .Length(8, 16)
                .WithMessage(ValidatorMessages.PASSWORD_INVALID)
                .Matches(PasswordRegex)
                .WithMessage(ValidatorMessages.PASSWORD_INVALID);

            RuleFor(x => x.PasswordConfirmation)
                .Equal(x => x.Password)
                .WithMessage(ValidatorMessages.PASSWORD_MISMATCH);

            RuleFor(c => c.Identity)
                .NotEmpty()
                .WithMessage(ValidatorMessages.IDENTITY_REQUIRED)
                .Length(11, 14)
                .WithMessage(ValidatorMessages.IDENTITY_INVALID)
                .Matches(@"^\d+$")
                .WithMessage(ValidatorMessages.IDENTITY_INVALID)
                .Must((request, identity) =>
                {
                    return request.Role switch
                    {
                        Role.User when identity.Length == 11 => true,
                        Role.Retailer when identity.Length == 14 => true,
                        _ => false
                    };
                })
                .WithMessage(ValidatorMessages.IDENTITY_INVALID);

            RuleFor(c => c.Role)
                .IsInEnum()
                .WithMessage(ValidatorMessages.ROLE_INVALID);

            RuleFor(c => c.FullName)
                .NotEmpty()
                .WithMessage(ValidatorMessages.NAME_REQUIRED)
                .Matches(NameRegex)
                .WithMessage(ValidatorMessages.NAME_INVALID);
        }
    }
}
