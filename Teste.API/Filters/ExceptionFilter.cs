using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Teste.Application.DTOs.Responses;
using Teste.Shared.Constants;
using Teste.Shared.Exceptions;

namespace Teste.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var errorMessages = new[] { Messages.UNKNOWN_ERROR };
        var statusCode = HttpStatusCode.InternalServerError;

        if (context.Exception is DetailedException detailedException)
        {
            errorMessages = detailedException.ErrorMessages.ToArray();
            statusCode = HttpStatusCode.BadRequest;
        }

        context.Result = new ObjectResult(new { data = new ExceptionOut(errorMessages) })
        {
            StatusCode = (int)statusCode
        };
    }
}