using Amazon.S3;
using Amazon.S3.Model;
using Application.Contracts.Armazenamento;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Armazenamento;

/// <summary>
/// Implementação do serviço de armazenamento de relatórios usando Amazon S3.
/// </summary>
public class S3ArmazenamentoArquivoService : IArmazenamentoArquivoService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3ArmazenamentoArquivoService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWS:S3:BucketName"] ?? throw new InvalidOperationException("Configuração do bucket S3 não encontrada");
    }

    public async Task<string> ArmazenarAsync(byte[] conteudo, string nomeArquivo, string contentType)
    {
        var key = $"relatorios/{nomeArquivo}";

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = new MemoryStream(conteudo),
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(putRequest);

        return $"s3://{_bucketName}/{key}";
    }
}
