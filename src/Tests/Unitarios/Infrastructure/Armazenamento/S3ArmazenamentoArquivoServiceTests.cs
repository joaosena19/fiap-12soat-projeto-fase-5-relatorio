using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure.Armazenamento;
using Microsoft.Extensions.Configuration;

namespace Tests.Infrastructure.Armazenamento;

public class S3ArmazenamentoArquivoServiceTests
{
    private readonly S3ArmazenamentoArquivoServiceTestFixture _fixture = new();

    [Fact(DisplayName = "Deve lançar exceção quando nome do bucket não está configurado")]
    [Trait("Infrastructure", "S3ArmazenamentoArquivoService")]
    public void Construtor_DeveLancarExcecao_QuandoBucketNaoConfigurado()
    {
        // Arrange
        var configuracaoVazia = new ConfigurationBuilder().Build();
        var loggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();

        // Act
        var acao = () => new S3ArmazenamentoArquivoService(_fixture.S3ClientMock.Object, configuracaoVazia, loggerFactoryMock.Object);

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve armazenar arquivo e retornar URL pré-assinada quando upload bem-sucedido")]
    [Trait("Infrastructure", "S3ArmazenamentoArquivoService")]
    public async Task ArmazenarAsync_DeveRetornarUrlPreAssinada_QuandoUploadBemSucedido()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var conteudo = new byte[] { 1, 2, 3 };
        var nomeArquivo = $"{analiseDiagramaId}/relatorio.json";
        _fixture.S3ClientMock.AoSalvarObjeto().Retorna();

        // Act
        var url = await _fixture.ArmazenarAsync(analiseDiagramaId, conteudo, nomeArquivo, "application/json");

        // Assert
        url.ShouldNotBeNullOrWhiteSpace();
        url.ShouldStartWith("https://");
        url.ShouldBe(S3ArmazenamentoArquivoServiceTestFixture.UrlPreAssinadaPadrao);
    }

    [Fact(DisplayName = "Deve propagar exceção quando upload S3 falha")]
    [Trait("Infrastructure", "S3ArmazenamentoArquivoService")]
    public async Task ArmazenarAsync_DevePropagarExcecao_QuandoS3FalhaNoUpload()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var conteudo = new byte[] { 1, 2, 3 };
        _fixture.S3ClientMock.AoSalvarObjeto().LancaExcecao(new AmazonS3Exception("Erro de upload"));

        // Act
        var acao = () => _fixture.ArmazenarAsync(analiseDiagramaId, conteudo, "relatorio.json", "application/json");

        // Assert
        await acao.ShouldThrowAsync<AmazonS3Exception>();
    }

    [Fact(DisplayName = "Deve chamar PutObjectAsync com bucket e key corretos")]
    [Trait("Infrastructure", "S3ArmazenamentoArquivoService")]
    public async Task ArmazenarAsync_DeveChamarS3ComParametrosCorretos()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var conteudo = new byte[] { 10, 20, 30 };
        var nomeArquivo = $"{analiseDiagramaId}/relatorio.md";
        PutObjectRequest? requestCapturado = null;
        _fixture.S3ClientMock.AoSalvarObjeto().Retorna(callback: req => requestCapturado = req);

        // Act
        await _fixture.ArmazenarAsync(analiseDiagramaId, conteudo, nomeArquivo, "text/markdown");

        // Assert
        requestCapturado.ShouldNotBeNull();
        requestCapturado!.BucketName.ShouldBe("meu-bucket-teste");
        requestCapturado.Key.ShouldBe($"relatorios/{nomeArquivo}");
        requestCapturado.ContentType.ShouldBe("text/markdown");
    }
}
