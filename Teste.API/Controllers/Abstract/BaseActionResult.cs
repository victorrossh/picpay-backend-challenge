using System.Net;
using Microsoft.AspNetCore.Mvc;
using Teste.Application.DTOs.Responses;

namespace Teste.API.Controllers.Abstract;

public class BaseActionResult<T>(HttpStatusCode statusCode, T? data) : IActionResult
{
    private HttpStatusCode StatusCode { get; } = statusCode;
    private T? Data { get; } = data;

    public Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(new { statusCode = (int)StatusCode, data = Data })
        {
            StatusCode = (int)StatusCode
        };

        return objectResult.ExecuteResultAsync(context);
    }
}

public class BaseActionResult(HttpStatusCode statusCode, DefaultRes? data) : IActionResult
{
    private HttpStatusCode StatusCode { get; } = statusCode;
    private DefaultRes? Data { get; } = data;

    public Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(new { statusCode = (int)StatusCode, data = Data })
        {
            StatusCode = (int)StatusCode
        };

        return objectResult.ExecuteResultAsync(context);
    }
}