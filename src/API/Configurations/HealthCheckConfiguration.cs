using Infrastructure.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace API.Configurations;

/// <summary>
/// Configuração de verificações de saúde (health checks) da aplicação
/// </summary>
public static class HealthCheckConfiguration
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    private static readonly string[] LiveTags = ["live"];
    private static readonly string[] ReadyTags = ["ready"];

    /// <summary>
    /// Adiciona e configura os serviços de verificação de saúde da aplicação
    /// </summary>
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("A aplicação está viva."), tags: LiveTags)
            .AddDbContextCheck<AppDbContext>("database", HealthStatus.Unhealthy, tags: ReadyTags);

        return services;
    }

    /// <summary>
    /// Configura os endpoints de verificação de saúde da aplicação
    /// </summary>
    public static WebApplication UseHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = WriteHealthCheckResponse
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteHealthCheckResponse
        });

        app.MapHealthChecks("/health/startup", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteHealthCheckResponse
        });

        return app;
    }

    private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration
            }),
            totalDuration = report.TotalDuration
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonSerializerOptions));
    }
}
