using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Teste.Infrastructure.Contexts;

namespace Teste.Infrastructure;

public static class InfrastructureInjection
{
    public static async Task AddInfrastructureInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddContexts(configuration);

        await Task.CompletedTask;
    }

    private static void AddContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseMySQL(configuration["Connections:MySQL"]!));
    }
}