namespace Tests.Infrastructure.Handlers;

public class ResultadoDiagramaHandlerTests
{
    private readonly ResultadoDiagramaHandlerTestFixture _fixture = new();

    [Fact(DisplayName = "BuscarPorAnaliseDiagramaIdAsync deve chamar presenter com sucesso quando resultado existe")]
    [Trait("Infrastructure", "ResultadoDiagramaHandler")]
    public async Task BuscarPorAnaliseDiagramaIdAsync_DeveChamarPresenter_QuandoResultadoExiste()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Build();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);

        // Act
        await _fixture.BuscarPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        // Assert
        _fixture.BuscarPresenterMock.DeveTerApresentadoSucesso();
    }

    [Fact(DisplayName = "BuscarPorAnaliseDiagramaIdAsync deve apresentar erro quando resultado não existe")]
    [Trait("Infrastructure", "ResultadoDiagramaHandler")]
    public async Task BuscarPorAnaliseDiagramaIdAsync_DeveApresentarErro_QuandoResultadoNaoExiste()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).NaoRetornaNada();

        // Act
        await _fixture.BuscarPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        // Assert
        _fixture.BuscarPresenterMock.DeveTerApresentadoErro(ErrorType.ResourceNotFound);
    }

    [Fact(DisplayName = "SolicitarGeracaoRelatoriosAsync deve executar use case")]
    [Trait("Infrastructure", "ResultadoDiagramaHandler")]
    public async Task SolicitarGeracaoRelatoriosAsync_DeveExecutarUseCase()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().Build();
        var tiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json };
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);
        _fixture.GatewayMock.AoSalvar().RetornaMesmoObjeto();
        _fixture.MessagePublisherMock.AoPublicarSolicitacaoGeracaoNaoFazNada();

        // Act
        await _fixture.SolicitarGeracaoRelatoriosAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        _fixture.SolicitarPresenterMock.DeveTerApresentadoSucesso();
        _fixture.SolicitarPresenterMock.NaoDeveTerApresentadoErro();
        _fixture.GatewayMock.DeveTerSalvo();
        _fixture.MessagePublisherMock.DeveTerPublicadoSolicitacaoGeracaoComTipos(TipoRelatorioEnum.Json);
    }
}
