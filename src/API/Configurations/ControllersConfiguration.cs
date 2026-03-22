using System.Text.Json.Serialization;

namespace API.Configurations;

/// <summary>
/// Configuração de controllers
/// </summary>
public static class ControllersConfiguration
{
    /// <summary>
    /// Configura os controllers com autorização obrigatória e conversão de enums
    /// </summary>
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
            {
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }
}
