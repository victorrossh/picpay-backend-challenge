using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teste.API.Controllers.Abstract;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;
using Teste.Shared;

namespace Teste.API.Controllers;

[ApiController]
[Route("api/v1/wallet")]
[Authorize]
public class WalletController(IWalletImp wallet) : ControllerBase
{
    [HttpGet("balance")]
    public async Task<BaseActionResult<BalanceRes, Error>> BalanceRequest(CancellationToken cancellationToken)
    {
        var result = await wallet.BalanceAsync(cancellationToken);

        return new BaseActionResult<BalanceRes, Error>(
            result.IsSuccess
                ? HttpStatusCode.OK
                : result.Error is NotFoundError
                    ? HttpStatusCode.NotFound
                    : HttpStatusCode.BadRequest,
            result,
            HttpContext.Request.Path,
            HttpContext.Request.Method,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier
        );
    }

    [HttpPost("transfer")]
    public async Task<BaseActionResult<TransferRes, Error>> TransferRequest([FromBody] TransferReq request,
        CancellationToken cancellationToken)
    {
        var result = await wallet.TransferAsync(request, cancellationToken);

        return new BaseActionResult<TransferRes, Error>(
            result.IsSuccess
                ? HttpStatusCode.OK
                : result.Error is NotFoundError
                    ? HttpStatusCode.NotFound
                    : HttpStatusCode.BadRequest,
            result,
            HttpContext.Request.Path,
            HttpContext.Request.Method,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier
        );
    }
}