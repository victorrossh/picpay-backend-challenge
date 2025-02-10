using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teste.API.Controllers.Abstract;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;

namespace Teste.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[AllowAnonymous]
public class AccountController(
    ISignUpImp signUp,
    ISignInImp signIn) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<BaseActionResult> SignUpRequest([FromBody] SignUpReq request,
        CancellationToken cancellationToken)
    {
        return new BaseActionResult(
            HttpStatusCode.OK,
            await signUp.ExecuteSignUpAsync(request, cancellationToken));
    }

    [HttpPost("signin")]
    public async Task<BaseActionResult<TokenRes>> SignInRequest([FromBody] SignInReq request,
        CancellationToken cancellationToken)
    {
        return new BaseActionResult<TokenRes>(
            HttpStatusCode.OK,
            await signIn.ExecuteSignInAsync(request, cancellationToken));
    }
}