using Infrastructure.Relatorios;
using System.Text.Json;

namespace Tests.Infrastructure.Relatorios;

public class RelatorioJsonStrategyTests
{
    private readonly RelatorioJsonStrategy _strategy;

    public RelatorioJsonStrategyTests()
    {
        var loggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();
        _strategy = new RelatorioJsonStrategy(loggerFactoryMock.Object);
    }

    [Fact(DisplayName = "TipoRelatorio deve ser Json")]
    [Trait("Infrastructure", "RelatorioJsonStrategy")]
    public void TipoRelatorio_DeveSerJson()
    {
        // Assert
        _strategy.TipoRelatorio.ShouldBe(TipoRelatorioEnum.Json);
    }

    [Fact(DisplayName = "GerarAsync deve produzir JSON válido com todos os campos da análise")]
    [Trait("Infrastructure", "RelatorioJsonStrategy")]
    public async Task GerarAsync_DeveProduzirJsonValido_ComTodosOsCampos()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();

        // Act
        var conteudos = await _strategy.GerarAsync(resultadoDiagrama);

        // Assert
        conteudos.ShouldNotBeNull();
        var jsonString = conteudos.ObterValor("jsonString");
        jsonString.ShouldNotBeNullOrWhiteSpace();

        var json = JsonSerializer.Deserialize<JsonElement>(jsonString!);
        json.GetProperty("DescricaoAnalise").GetString().ShouldNotBeNullOrWhiteSpace();
        json.GetProperty("ComponentesIdentificados").GetArrayLength().ShouldBeGreaterThan(0);
        json.GetProperty("RiscosArquiteturais").GetArrayLength().ShouldBeGreaterThan(0);
        json.GetProperty("RecomendacoesBasicas").GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "GerarAsync deve lançar exceção quando análise não disponível")]
    [Trait("Infrastructure", "RelatorioJsonStrategy")]
    public async Task GerarAsync_DeveLancarExcecao_QuandoAnaliseNaoDisponivel()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Build();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() => _strategy.GerarAsync(resultadoDiagrama));
    }
}
