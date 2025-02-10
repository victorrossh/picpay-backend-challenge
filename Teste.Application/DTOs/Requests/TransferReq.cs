namespace Teste.Application.DTOs.Requests;

public record TransferReq(
    string payeeId,
    decimal amount
);