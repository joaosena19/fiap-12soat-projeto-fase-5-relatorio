using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;

namespace API.Configurations;

/// <summary>
/// Configuração de monitoramento e observabilidade.
/// </summary>
public static class MonitoringConfiguration
{
    /// <summary>
    /// Registra serviços de monitoramento (correlation ID, métricas, etc).
    /// </summary>
    public static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

        return services;
    }
}
