namespace Teste.Application.DTOs.Requests;

public record SignUpReq(
    string name,
    string identity,
    string email,
    string password);