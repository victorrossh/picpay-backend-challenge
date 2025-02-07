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
        var objectResult = new ObjectResult(new { data = Data })
        {
            StatusCode = (int)StatusCode
        };

        return objectResult.ExecuteResultAsync(context);
    }
}

public class BaseActionResult(HttpStatusCode statusCode, DefaultOut? data) : IActionResult
{
    private HttpStatusCode StatusCode { get; } = statusCode;
    private DefaultOut? Data { get; } = data;

    public Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(new { data = Data })
        {
            StatusCode = (int)StatusCode
        };

        return objectResult.ExecuteResultAsync(context);
    }
}