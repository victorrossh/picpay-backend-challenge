using Teste.Domain.Enums;

namespace Teste.Application.DTOs.Requests;

public record SignUpAccountIn(
    string FullName,
    string Identity,
    string Email,
    string Password,
    string PasswordConfirmation,
    Role Role);