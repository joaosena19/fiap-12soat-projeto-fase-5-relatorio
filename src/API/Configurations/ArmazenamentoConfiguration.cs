using Amazon.S3;
using Application.Contracts.Armazenamento;
using Infrastructure.Armazenamento;

namespace API.Configurations;

/// <summary>
/// Configuração de serviços de armazenamento.
/// </summary>
public static class ArmazenamentoConfiguration
{
    /// <summary>
    /// Configura o Amazon S3 e serviços de armazenamento de relatórios.
    /// </summary>
    public static IServiceCollection AddArmazenamento(this IServiceCollection services, IConfiguration configuration)
    {
        var nomeRegiao = configuration["AWS:Region"] ?? throw new InvalidOperationException("Configuração AWS:Region não encontrada");
        var region = Amazon.RegionEndpoint.GetBySystemName(nomeRegiao);

        var accessKey = configuration["AWS:AccessKeyId"];
        var secretKey = configuration["AWS:SecretAccessKey"];

        if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(accessKey, secretKey, region));
        else
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(region));

        services.AddScoped<IArmazenamentoArquivoService, S3ArmazenamentoArquivoService>();

        return services;
    }
}
