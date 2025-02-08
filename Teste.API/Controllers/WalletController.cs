using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teste.API.Controllers.Abstract;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;

namespace Teste.API.Controllers;

[ApiController]
[Route("api/v1/{controller}")]
[Authorize]
public class WalletController(IWalletImp wallet) : ControllerBase
{
    [HttpGet("balance")]
    public async Task<BaseActionResult<BalanceOut>> BalanceRequest(CancellationToken cancellationToken)
    {
        var accountId = HttpContext.Items["AccountId"] as string;
        if (string.IsNullOrEmpty(accountId)) return new BaseActionResult<BalanceOut>(HttpStatusCode.Unauthorized, null);

        return new BaseActionResult<BalanceOut>(
            HttpStatusCode.OK,
            await wallet.GetBalanceAsync(accountId, cancellationToken));
    }
}