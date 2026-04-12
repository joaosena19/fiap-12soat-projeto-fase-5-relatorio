using Tests.Helpers.Fixtures;
using Aggregates = global::Domain.ResultadoDiagrama.Aggregates;

namespace Tests.Features.ResultadoDiagrama.ListarResultados;

public class ListarResultadosDiagramaUseCaseTests
{
    private readonly ListarResultadosDiagramaUseCaseTestFixture _fixture = new();

    #region Cenario Feliz

    [Fact(DisplayName = "Deve apresentar sucesso com lista de resultados")]
    [Trait("UseCase", "ListarResultadosDiagrama")]
    public async Task ExecutarAsync_DeveApresentarSucesso_QuandoExistemResultados()
    {
        // Arrange
        var resultados = new List<Aggregates.ResultadoDiagrama>
        {
            new ResultadoDiagramaBuilder().Build(),
            new ResultadoDiagramaBuilder().Analisado().Build()
        };
        _fixture.GatewayMock.AoListar().Retorna(resultados);

        // Act
        await _fixture.ExecutarAsync();

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoSucesso();
        _fixture.PresenterMock.NaoDeveTerApresentadoErro();
    }

    [Fact(DisplayName = "Deve apresentar sucesso com lista vazia quando não há resultados")]
    [Trait("UseCase", "ListarResultadosDiagrama")]
    public async Task ExecutarAsync_DeveApresentarSucessoComListaVazia_QuandoNaoExistemResultados()
    {
        // Arrange
        _fixture.GatewayMock.AoListar().Retorna(new List<Aggregates.ResultadoDiagrama>());

        // Act
        await _fixture.ExecutarAsync();

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoSucesso();
        _fixture.PresenterMock.NaoDeveTerApresentadoErro();
    }

    #endregion

    #region Erro Inesperado

    [Fact(DisplayName = "Deve apresentar erro inesperado quando ocorre exceção")]
    [Trait("UseCase", "ListarResultadosDiagrama")]
    public async Task ExecutarAsync_DeveApresentarErroInesperado_QuandoExcecaoNaoPrevista()
    {
        // Arrange
        _fixture.GatewayMock.AoListar().LancaExcecao(new InvalidOperationException("Erro de infra"));

        // Act
        await _fixture.ExecutarAsync();

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.UnexpectedError);
        _fixture.PresenterMock.NaoDeveTerApresentadoSucesso();
        _fixture.LoggerMock.DeveTerLogadoErrorComException();
    }

    #endregion
}
