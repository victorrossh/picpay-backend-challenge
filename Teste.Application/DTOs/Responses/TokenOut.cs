namespace Teste.Application.DTOs.Responses;

public record TokenOut(string Token, string RequestId, DateTime Expiry);