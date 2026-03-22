namespace API.Configurations;

/// <summary>
/// Configuração de cabeçalhos de segurança
/// </summary>
public static class SecurityConfiguration
{
    /// <summary>
    /// Configura os cabeçalhos de segurança da aplicação
    /// </summary>
    public static IApplicationBuilder UseSecurityHeadersConfiguration(this IApplicationBuilder app)
    {
        app.UseSecurityHeaders(policy =>
        {
            policy.AddDefaultSecurityHeaders();
            policy.AddContentSecurityPolicy(builder =>
            {
                builder.AddScriptSrc().Self();
            });
        });

        return app;
    }
}
