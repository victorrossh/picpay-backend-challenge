using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teste.API.Controllers.Abstract;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;

namespace Teste.API.Controllers;

[ApiController]
[Route("api/v1/wallet")]
[Authorize]
public class WalletController(IWalletImp wallet) : ControllerBase
{
    [HttpGet("balance")]
    public async Task<BaseActionResult<BalanceRes>> BalanceRequest(CancellationToken cancellationToken)
    {
        return new BaseActionResult<BalanceRes>(
            HttpStatusCode.OK,
            await wallet.BalanceAsync(HttpContext.Items["AccountId"]!.ToString(), cancellationToken));
    }

    [HttpPost("transfer")]
    public async Task<BaseActionResult<TransferRes>> TransferRequest([FromBody] TransferReq request,
        CancellationToken cancellationToken)
    {
        return new BaseActionResult<TransferRes>(
            HttpStatusCode.OK,
            await wallet.TransferAsync(HttpContext.Items["AccountId"]!.ToString()!, request, cancellationToken));
    }
}