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
[Route("api/v1/auth")]
[AllowAnonymous]
public class AccountController(
    ISignUpImp signUp,
    ISignInImp signIn) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<BaseActionResult<DefaultRes, Error>> SignUpRequest([FromBody] SignUpReq request,
        CancellationToken cancellationToken)
    {
        var result = await signUp.ExecuteSignUpAsync(request, cancellationToken);

        return new BaseActionResult<DefaultRes, Error>(
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

    [HttpPost("signin")]
    public async Task<BaseActionResult<TokenRes, Error>> SignInRequest([FromBody] SignInReq request,
        CancellationToken cancellationToken)
    {
        var result = await signIn.ExecuteSignInAsync(request, cancellationToken);

        return new BaseActionResult<TokenRes, Error>(
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