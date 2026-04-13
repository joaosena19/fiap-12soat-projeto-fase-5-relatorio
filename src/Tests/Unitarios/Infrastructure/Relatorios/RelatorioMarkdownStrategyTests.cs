using Infrastructure.Relatorios;

namespace Tests.Infrastructure.Relatorios;

public class RelatorioMarkdownStrategyTests
{
    private readonly RelatorioStrategyTestFixture<RelatorioMarkdownStrategy> _fixture = new((armazenamento, loggerFactory) => new RelatorioMarkdownStrategy(armazenamento, loggerFactory));

    [Fact(DisplayName = "TipoRelatorio deve ser Markdown")]
    [Trait("Infrastructure", "RelatorioMarkdownStrategy")]
    public void TipoRelatorio_DeveSerMarkdown()
    {
        // Assert
        _fixture.Strategy.TipoRelatorio.ShouldBe(TipoRelatorioEnum.Markdown);
    }

    [Fact(DisplayName = "GerarAsync deve produzir conteúdo markdown e chamar armazenamento")]
    [Trait("Infrastructure", "RelatorioMarkdownStrategy")]
    public async Task GerarAsync_DeveProduzirMarkdown_EChamarArmazenamento()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
        var urlEsperada = "https://s3.amazonaws.com/bucket/relatorio.md";
        _fixture.ArmazenamentoMock.AoArmazenar().Retorna(urlEsperada, contentType: "text/markdown; charset=utf-8");

        // Act
        var conteudos = await _fixture.Strategy.GerarAsync(resultadoDiagrama);

        // Assert
        conteudos.ShouldNotBeNull();
        var inlineMarkdown = conteudos.ObterValor("inlineMarkdown");
        inlineMarkdown.ShouldNotBeNullOrWhiteSpace();
        inlineMarkdown.ShouldContain("# Relatório Técnico");
        inlineMarkdown.ShouldContain("## Componentes Identificados");
        inlineMarkdown.ShouldContain("## Riscos Arquiteturais");
        inlineMarkdown.ShouldContain("## Recomendações Básicas");

        var url = conteudos.ObterValor("url");
        url.ShouldBe(urlEsperada);

        _fixture.ArmazenamentoMock.DeveTerArmazenado(contentType: "text/markdown; charset=utf-8");
    }

    [Fact(DisplayName = "GerarAsync deve propagar exceção quando armazenamento falha")]
    [Trait("Infrastructure", "RelatorioMarkdownStrategy")]
    public async Task GerarAsync_DevePropagarExcecao_QuandoArmazenamentoFalha()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
        _fixture.ArmazenamentoMock.AoArmazenar().LancaExcecao(new InvalidOperationException("Falha no S3"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() => _fixture.Strategy.GerarAsync(resultadoDiagrama));
    }
}
