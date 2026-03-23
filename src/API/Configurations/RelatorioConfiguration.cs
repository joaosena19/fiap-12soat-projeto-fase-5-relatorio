using Application.Contracts.Messaging;
using Application.Contracts.Relatorios;
using Infrastructure.Messaging.Publishers;
using Infrastructure.Relatorios;

namespace API.Configurations;

public static class RelatorioConfiguration
{
    public static IServiceCollection AddRelatorios(this IServiceCollection services)
    {
        services.AddScoped<IRelatorioMessagePublisher, RelatorioMessagePublisher>();
        services.AddScoped<IRelatorioStrategy, RelatorioMarkdownStrategy>();
        services.AddScoped<IRelatorioStrategy, RelatorioPdfStrategy>();
        services.AddScoped<IRelatorioStrategy, RelatorioJsonStrategy>();
        services.AddScoped<IRelatorioStrategyResolver, RelatorioStrategyResolver>();

        return services;
    }
}
