using Amazon.S3;
using Application.Contracts.Armazenamento;
using Infrastructure.Armazenamento;
using Microsoft.Extensions.Configuration;

namespace Tests.Infrastructure.Armazenamento;

public class S3ArmazenamentoArquivoServiceTestFixture
{
    public Mock<IAmazonS3> S3ClientMock { get; } = new();
    public IArmazenamentoArquivoService Service { get; }

    public S3ArmazenamentoArquivoServiceTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AWS:S3:BucketName"] = "meu-bucket-teste" })
            .Build();

        var loggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();
        Service = new S3ArmazenamentoArquivoService(S3ClientMock.Object, configuration, loggerFactoryMock.Object);
    }

    public async Task<string> ArmazenarAsync(Guid analiseDiagramaId, byte[] conteudo, string nomeArquivo, string contentType) => await Service.ArmazenarAsync(analiseDiagramaId, conteudo, nomeArquivo, contentType);
}
