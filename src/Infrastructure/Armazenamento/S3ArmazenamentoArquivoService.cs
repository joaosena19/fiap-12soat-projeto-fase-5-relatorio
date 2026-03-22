using Amazon.S3;
using Amazon.S3.Model;
using Application.Contracts.Armazenamento;
using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using System.Diagnostics;

namespace Infrastructure.Armazenamento;

/// <summary>
/// Implementação do serviço de armazenamento de relatórios usando Amazon S3.
/// </summary>
public class S3ArmazenamentoArquivoService : IArmazenamentoArquivoService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly IAppLogger _logger;

    public S3ArmazenamentoArquivoService(IAmazonS3 s3Client, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWS:S3:BucketName"] ?? throw new InvalidOperationException("Configuração do bucket S3 não encontrada");
        _logger = new LoggerAdapter<S3ArmazenamentoArquivoService>(loggerFactory.CreateLogger<S3ArmazenamentoArquivoService>());
    }

    public async Task<string> ArmazenarAsync(Guid analiseDiagramaId, byte[] conteudo, string nomeArquivo, string contentType)
    {
        var key = $"relatorios/{nomeArquivo}";
        var cronometro = Stopwatch.StartNew();

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = new MemoryStream(conteudo),
            ContentType = contentType
        };

        try
        {
            _logger.LogDebug($"Iniciando armazenamento do relatório {{{LogNomesPropriedades.NomeArquivo}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", nomeArquivo, analiseDiagramaId);

            await _s3Client.PutObjectAsync(putRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Falha ao armazenar relatório {{{LogNomesPropriedades.NomeArquivo}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", nomeArquivo, analiseDiagramaId);
            throw;
        }

        _logger.LogDebug($"Armazenamento do relatório concluído para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms com {{{LogNomesPropriedades.Tamanho}}} bytes", analiseDiagramaId, cronometro.ElapsedMilliseconds, conteudo.Length);

        return $"s3://{_bucketName}/{key}";
    }
}
