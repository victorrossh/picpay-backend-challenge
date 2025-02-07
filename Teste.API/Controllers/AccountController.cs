using System.Net;
using Microsoft.AspNetCore.Mvc;
using Teste.API.Controllers.Abstract;
using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.UseCases.Implementations;

namespace Teste.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AccountController(
    ISignUpImp signUp,
    ISignInImp signIn) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<BaseActionResult> SignUpRequest([FromBody] SignUpAccountIn request,
        CancellationToken cancellationToken)
    {
        return new BaseActionResult(
            HttpStatusCode.OK,
            await signUp.ExecuteSignUpAsync(request, cancellationToken));
    }

    [HttpPost("signin")]
    public async Task<BaseActionResult<TokenOut>> SignInRequest([FromBody] SignInAccountIn request,
        CancellationToken cancellationToken)
    {
        return new BaseActionResult<TokenOut>(
            HttpStatusCode.OK,
            await signIn.ExecuteSignInAsync(request, cancellationToken));
    }
}