using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Teste.Infrastructure;

namespace Teste.Application;

public static class ApplicationInjection
{
    public static async Task AddApplicationInjection(this IServiceCollection services, IConfiguration configuration)
    {
        await services.AddInfrastructureInjection(configuration);
    }
}