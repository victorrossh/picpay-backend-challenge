using System.Text.RegularExpressions;
using FluentValidation;
using Teste.Application.DTOs.Requests;
using Teste.Shared.Constants;

namespace Teste.Application.UseCases.Validators;

public class SignInValidator : AbstractValidator<SignInAccountIn>
{
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$");

    public SignInValidator()
    {
        // Password validation rules
        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage(Messages.PASSWORD_REQUIRED)
            .MinimumLength(8)
            .WithMessage(Messages.PASSWORD_MIN_LENGTH)
            .MaximumLength(16)
            .WithMessage(Messages.PASSWORD_MAX_LENGTH)
            .Matches(PasswordRegex)
            .WithMessage(Messages.PASSWORD_INVALID);

        // Email validation rules
        RuleFor(e => e.Email)
            .NotEmpty()
            .WithMessage(Messages.EMAIL_REQUIRED) // Use a more appropriate error message for empty email
            .EmailAddress()
            .WithMessage(Messages.EMAIL_INVALID);
    }
}