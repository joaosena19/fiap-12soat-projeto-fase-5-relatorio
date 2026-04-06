using Tests.Helpers.Fixtures;

namespace Tests.Features.ResultadoDiagrama.BuscarPorId;

public class BuscarResultadoDiagramaPorIdUseCaseTests
{
    private readonly BuscarResultadoDiagramaPorIdUseCaseTestFixture _fixture;

    public BuscarResultadoDiagramaPorIdUseCaseTests()
    {
        _fixture = new BuscarResultadoDiagramaPorIdUseCaseTestFixture();
    }

    [Fact(DisplayName = "Deve apresentar sucesso quando resultado existe")]
    [Trait("UseCase", "BuscarResultadoDiagramaPorId")]
    public async Task ExecutarAsync_DeveApresentarSucesso_QuandoResultadoExiste()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Build();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoSucesso();
        _fixture.PresenterMock.NaoDeveTerApresentadoErro();
    }

    [Fact(DisplayName = "Deve apresentar erro quando resultado não existe")]
    [Trait("UseCase", "BuscarResultadoDiagramaPorId")]
    public async Task ExecutarAsync_DeveApresentarErro_QuandoResultadoNaoExiste()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).NaoRetornaNada();

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.ResourceNotFound);
        _fixture.PresenterMock.NaoDeveTerApresentadoSucesso();
    }

    [Fact(DisplayName = "Deve apresentar erro de domínio quando domain exception ocorre")]
    [Trait("UseCase", "BuscarResultadoDiagramaPorId")]
    public async Task ExecutarAsync_DeveApresentarErro_QuandoDomainExceptionOcorre()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).LancaExcecao(new DomainException("erro de domínio", ErrorType.InvalidInput));

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.InvalidInput);
        _fixture.LoggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "Deve apresentar erro inesperado quando exceção não prevista ocorre")]
    [Trait("UseCase", "BuscarResultadoDiagramaPorId")]
    public async Task ExecutarAsync_DeveApresentarErroInesperado_QuandoExcecaoNaoPrevista()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).LancaExcecao(new InvalidOperationException("erro inesperado"));

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.UnexpectedError);
        _fixture.LoggerMock.DeveTerLogadoErrorComException();
    }
}