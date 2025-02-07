using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Teste.Domain.Repositories;
using Teste.Infrastructure.Contexts;
using Teste.Infrastructure.Repositories;
using Teste.Shared.Utilities;

namespace Teste.Infrastructure;

public static class InfrastructureInjection
{
    public static async Task AddInfrastructureInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TesteDbContext>(options =>
            options.UseSqlServer(configuration.GetConfiguration<string>("Connections:SqlServer")));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();

        await Task.CompletedTask;
    }
}