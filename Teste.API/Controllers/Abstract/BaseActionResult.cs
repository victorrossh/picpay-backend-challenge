using System.Net;
using Microsoft.AspNetCore.Mvc;
using Teste.Shared;

namespace Teste.API.Controllers.Abstract;

public class BaseActionResult<T, E>(
    HttpStatusCode statusCode,
    Result<T, E> result,
    string path,
    string method,
    string traceIdentifier)
    : IActionResult
    where E : Error
{
    private HttpStatusCode StatusCode { get; } = statusCode;
    private Result<T, E> Result { get; } = result;
    private string Path { get; } = path;
    private string Method { get; } = method;
    private string TraceIdentifier { get; } = traceIdentifier;

    public Task ExecuteResultAsync(ActionContext context)
    {
        var response = new
        {
            statusCode = (int)StatusCode,
            data = Result.IsSuccess
                ? new
                {
                    result = (object?)Result.Value, errors = new List<string>() as IReadOnlyList<string>
                }
                : new
                {
                    result = (object?)null, errors = Result.Error?.Messages ?? new List<string>()
                },
            instance = Path,
            method = Method,
            timestamp = DateTime.UtcNow.ToString("o"),
            traceId = TraceIdentifier
        };

        var objectResult = new ObjectResult(response)
        {
            StatusCode = (int)StatusCode
        };

        return objectResult.ExecuteResultAsync(context);
    }
}