using System.Text.RegularExpressions;
using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases.Validators;

public class SignInValidator : AbstractValidator<SignInAccountIn>
{
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$",
        RegexOptions.None, TimeSpan.FromSeconds(1));

    public SignInValidator()
    {
        // Password validation rules
        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage(ValidatorMessages.PASSWORD_REQUIRED)
            .Length(8, 16)
            .WithMessage(ValidatorMessages.PASSWORD_INVALID)
            .Matches(PasswordRegex)
            .WithMessage(ValidatorMessages.PASSWORD_INVALID);

        // Email validation rules
        RuleFor(e => e.Email)
            .NotEmpty()
            .WithMessage(ValidatorMessages.EMAIL_REQUIRED) 
            .EmailAddress()
            .WithMessage(ValidatorMessages.EMAIL_INVALID);
    }
}