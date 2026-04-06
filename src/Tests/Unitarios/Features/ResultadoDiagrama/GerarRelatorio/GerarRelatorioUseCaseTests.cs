using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using Tests.Helpers.Fixtures;

namespace Tests.Features.ResultadoDiagrama.GerarRelatorio;

public class GerarRelatorioUseCaseTests
{
    private readonly GerarRelatorioUseCaseTestFixture _fixture;

    public GerarRelatorioUseCaseTests()
    {
        _fixture = new GerarRelatorioUseCaseTestFixture();
    }

    [Fact(DisplayName = "Deve concluir relatório com sucesso quando análise está disponível")]
    [Trait("UseCase", "GerarRelatorio")]
    public async Task ExecutarAsync_DeveConcluirRelatorioComSucesso_QuandoAnaliseDisponivel()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().ComRelatorioSolicitado(TipoRelatorioEnum.Markdown).Build();
        var conteudos = Conteudos.Criar(new Dictionary<string, string> { ["conteudo"] = "resultado" });

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);
        _fixture.StrategyMock.AoGerar().Retorna(conteudos);

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, TipoRelatorioEnum.Markdown);

        // Assert
        _fixture.GatewayMock.DeveTerSalvo(2);
        _fixture.MetricsMock.DeveTerRegistradoRelatorioGerado();
        _fixture.MetricsMock.NaoDeveTerRegistradoRelatorioComFalha();
    }

    [Fact(DisplayName = "Deve retornar silenciosamente quando resultado não existe")]
    [Trait("UseCase", "GerarRelatorio")]
    public async Task ExecutarAsync_DeveRetornarSilenciosamente_QuandoResultadoNaoExiste()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).NaoRetornaNada();

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, TipoRelatorioEnum.Markdown);

        // Assert
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MetricsMock.NaoDeveTerRegistradoRelatorioGerado();
    }

    [Fact(DisplayName = "Deve retornar silenciosamente quando relatório não pode ser gerado")]
    [Trait("UseCase", "GerarRelatorio")]
    public async Task ExecutarAsync_DeveRetornarSilenciosamente_QuandoRelatorioNaoPodeSerGerado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().ComRelatorioEmProcessamento(TipoRelatorioEnum.Markdown).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, TipoRelatorioEnum.Markdown);

        // Assert
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MetricsMock.NaoDeveTerRegistradoRelatorioGerado();
    }

    [Fact(DisplayName = "Deve retornar silenciosamente quando análise não está disponível")]
    [Trait("UseCase", "GerarRelatorio")]
    public async Task ExecutarAsync_DeveRetornarSilenciosamente_QuandoAnaliseNaoDisponivel()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, TipoRelatorioEnum.Markdown);

        // Assert
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MetricsMock.NaoDeveTerRegistradoRelatorioGerado();
    }

    [Fact(DisplayName = "Deve registrar falha quando strategy lança exceção")]
    [Trait("UseCase", "GerarRelatorio")]
    public async Task ExecutarAsync_DeveRegistrarFalha_QuandoStrategyLancaExcecao()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().ComRelatorioSolicitado(TipoRelatorioEnum.Markdown).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);
        _fixture.StrategyMock.AoGerar().LancaExcecao(new InvalidOperationException("erro na strategy"));

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, TipoRelatorioEnum.Markdown);

        // Assert
        _fixture.GatewayMock.DeveTerSalvo(2);
        _fixture.MetricsMock.DeveTerRegistradoRelatorioComFalha();
        _fixture.MetricsMock.NaoDeveTerRegistradoRelatorioGerado();
        _fixture.LoggerMock.DeveTerLogadoErrorComException();
    }
}