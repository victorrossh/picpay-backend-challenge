using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Teste.Application.Services;
using Teste.Application.Services.Implementations;
using Teste.Application.UseCases;
using Teste.Application.UseCases.Implementations;
using Teste.Infrastructure;

namespace Teste.Application;

public static class ApplicationInjection
{
    public static async Task AddApplicationInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICryptographyImp, CryptographyService>();
        services.AddScoped<ITokenizationImp, TokenizationService>();

        services.AddScoped<ISignInImp, SignInUseCase>();
        services.AddScoped<ISignUpImp, SignUpUseCase>();
        services.AddScoped<IWalletImp, WalletUseCase>();

        await services.AddInfrastructureInjection(configuration);
    }
}