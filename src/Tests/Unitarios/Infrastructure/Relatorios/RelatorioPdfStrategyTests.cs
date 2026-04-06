using Infrastructure.Relatorios;
using QuestPDF.Infrastructure;

namespace Tests.Infrastructure.Relatorios;

public class RelatorioPdfStrategyTests
{
    private readonly RelatorioStrategyTestFixture<RelatorioPdfStrategy> _fixture = new((armazenamento, loggerFactory) => new RelatorioPdfStrategy(armazenamento, loggerFactory));

    [Fact(DisplayName = "TipoRelatorio deve ser Pdf")]
    [Trait("Infrastructure", "RelatorioPdfStrategy")]
    public void TipoRelatorio_DeveSerPdf()
    {
        // Assert
        _fixture.Strategy.TipoRelatorio.ShouldBe(TipoRelatorioEnum.Pdf);
    }

    [Fact(DisplayName = "GerarAsync deve armazenar PDF quando análise está disponível")]
    [Trait("Infrastructure", "RelatorioPdfStrategy")]
    public async Task GerarAsync_DeveArmazenarPdf_QuandoAnaliseDisponivel()
    {
        // Arrange
        QuestPDF.Settings.License = LicenseType.Community;
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
        _fixture.ArmazenamentoMock.AoArmazenar().Retorna("https://bucket/relatorio.pdf", contentType: "application/pdf");

        // Act
        var conteudos = await _fixture.Strategy.GerarAsync(resultadoDiagrama);

        // Assert
        conteudos.ObterValor("url").ShouldBe("https://bucket/relatorio.pdf");
        _fixture.ArmazenamentoMock.DeveTerArmazenado(contentType: "application/pdf");
    }

    [Fact(DisplayName = "GerarAsync deve propagar exceção quando armazenamento falha")]
    [Trait("Infrastructure", "RelatorioPdfStrategy")]
    public async Task GerarAsync_DevePropagarExcecao_QuandoArmazenamentoFalha()
    {
        // Arrange
        QuestPDF.Settings.License = LicenseType.Community;
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
        _fixture.ArmazenamentoMock.AoArmazenar().LancaExcecao(new InvalidOperationException("Falha no S3"), contentType: "application/pdf");

        // Act
        var acao = () => _fixture.Strategy.GerarAsync(resultadoDiagrama);

        // Assert
        await acao.ShouldThrowAsync<InvalidOperationException>();
    }
}