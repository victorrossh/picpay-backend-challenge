using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using Serilog;
using Teste.Shared.Exceptions;

namespace Teste.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    private readonly string? _requestId = Activity.Current?.Id ?? "Unknown";

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                ForbiddenException => HttpStatusCode.Forbidden,
                NotFoundException => HttpStatusCode.NotFound,
                BadRequestException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
            await HandleExceptionAsync(httpContext, ex, statusCode);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            messages = exception switch
            {
                BadRequestException badRequestEx => badRequestEx.Messages,
                ForbiddenException forbiddenEx => forbiddenEx.Messages,
                NotFoundException notFoundEx => notFoundEx.Messages,
                _ => [exception.Message]
            },
            traceId = context.TraceIdentifier,
            requestId = _requestId
        };

        Log.Error(exception, "An error occurred while processing the request");

        var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}