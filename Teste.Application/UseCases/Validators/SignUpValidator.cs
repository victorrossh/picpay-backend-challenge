using System.Text.RegularExpressions;
using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Domain.Enums;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases.Validators;

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
            .WithMessage(Messages.EMAIL_REQUIRED)
            .EmailAddress()
            .WithMessage(Messages.EMAIL_INVALID);

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage(Messages.PASSWORD_REQUIRED)
            .Length(8, 16)
            .WithMessage(Messages.PASSWORD_INVALID)
            .Matches(PasswordRegex)
            .WithMessage(Messages.PASSWORD_INVALID);

        RuleFor(c => c.Identity)
            .NotEmpty()
            .WithMessage(Messages.IDENTITY_REQUIRED)
            .Length(11, 14)
            .WithMessage(Messages.IDENTITY_INVALID)
            .Matches(@"^\d+$")
            .WithMessage(Messages.IDENTITY_INVALID);

        RuleFor(c => c.Identity)
            .NotEmpty()
            .WithMessage(Messages.IDENTITY_REQUIRED)
            .Must((request, identity) =>
            {
                switch (request.Role)
                {
                    case Role.User when identity.Length != 11:
                    case Role.Retailer when identity.Length != 14:
                        return false;
                    default:
                        return true;
                }
            })
            .WithMessage(Messages.IDENTITY_INVALID)
            .Matches(@"^\d+$")
            .WithMessage(Messages.IDENTITY_INVALID);

        RuleFor(c => c.Role)
            .IsInEnum()
            .WithMessage(Messages.INVALID_ROLE);

        RuleFor(c => c.FullName)
            .NotEmpty()
            .WithMessage(Messages.FULLNAME_REQUIRED)
            .Matches(NameRegex)
            .WithMessage(Messages.FULLNAME_INVALID);
    }
}