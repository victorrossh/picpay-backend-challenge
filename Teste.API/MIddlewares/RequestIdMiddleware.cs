using System.Diagnostics;

namespace Teste.API.MIddlewares;

public class RequestIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Items["RequestId"] = Activity.Current?.Id;

        await next(httpContext);
    }
}