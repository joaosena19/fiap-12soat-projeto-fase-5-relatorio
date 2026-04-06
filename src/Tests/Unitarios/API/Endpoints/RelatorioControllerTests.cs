using API.Dtos;
using API.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API.Endpoints;

public class RelatorioControllerTests : IDisposable
{
    private readonly RelatorioControllerTestFixture _fixture = new();

    public void Dispose() => _fixture.Dispose();

    #region BuscarPorAnaliseDiagramaId

    [Fact(DisplayName = "Deve retornar 404 quando ResultadoDiagrama não encontrado")]
    [Trait("API", "RelatorioController")]
    public async Task BuscarPorAnaliseDiagramaId_DeveRetornar404_QuandoNaoEncontrado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();

        // Act
        var resultado = await _fixture.Controller.BuscarPorAnaliseDiagramaId(analiseDiagramaId);

        // Assert
        var objectResult = resultado.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
    }

    [Fact(DisplayName = "Deve retornar 200 quando ResultadoDiagrama encontrado")]
    [Trait("API", "RelatorioController")]
    public async Task BuscarPorAnaliseDiagramaId_DeveRetornar200_QuandoEncontrado()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
        await _fixture.Contexto.ResultadosDiagrama.AddAsync(resultadoDiagrama);
        await _fixture.Contexto.SaveChangesAsync();

        // Act
        var resultado = await _fixture.Controller.BuscarPorAnaliseDiagramaId(resultadoDiagrama.AnaliseDiagramaId);

        // Assert
        resultado.ShouldBeOfType<OkObjectResult>();
    }

    #endregion

    #region SolicitarRelatorios

    [Fact(DisplayName = "Deve retornar 404 quando ResultadoDiagrama não encontrado ao solicitar relatórios")]
    [Trait("API", "RelatorioController")]
    public async Task SolicitarRelatorios_DeveRetornar404_QuandoNaoEncontrado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var request = new SolicitarRelatoriosRequestDto { TiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json } };

        // Act
        var resultado = await _fixture.Controller.SolicitarRelatorios(analiseDiagramaId, request);

        // Assert
        var objectResult = resultado.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
    }

    [Fact(DisplayName = "Deve retornar resultado de solicitação quando ResultadoDiagrama encontrado")]
    [Trait("API", "RelatorioController")]
    public async Task SolicitarRelatorios_DeveRetornarResultado_QuandoEncontrado()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
        await _fixture.Contexto.ResultadosDiagrama.AddAsync(resultadoDiagrama);
        await _fixture.Contexto.SaveChangesAsync();

        var request = new SolicitarRelatoriosRequestDto { TiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json } };
        _fixture.MessagePublisherMock.AoPublicarSolicitacaoGeracaoNaoFazNada();

        // Act
        var resultado = await _fixture.Controller.SolicitarRelatorios(resultadoDiagrama.AnaliseDiagramaId, request);

        // Assert — relatório Analisado e não solicitado → AceitoParaGeracao → HTTP 202
        var objectResult = resultado.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(202);
    }

    [Fact(DisplayName = "Deve retornar resultado com lista vazia de tipos de relatório")]
    [Trait("API", "RelatorioController")]
    public async Task SolicitarRelatorios_DeveRetornarResultado_QuandoListaVazia()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
        await _fixture.Contexto.ResultadosDiagrama.AddAsync(resultadoDiagrama);
        await _fixture.Contexto.SaveChangesAsync();

        var request = new SolicitarRelatoriosRequestDto { TiposRelatorio = new List<TipoRelatorioEnum>() };

        // Act
        var resultado = await _fixture.Controller.SolicitarRelatorios(resultadoDiagrama.AnaliseDiagramaId, request);

        // Assert — lista vazia é entrada inválida, use case retorna InvalidInput → HTTP 400
        var objectResult = resultado.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(400);
    }

    #endregion

    #region SolicitarRelatoriosRequestDto

    [Fact(DisplayName = "SolicitarRelatoriosRequestDto deve inicializar lista vazia")]
    [Trait("API", "SolicitarRelatoriosRequestDto")]
    public void SolicitarRelatoriosRequestDto_DeveInicializarListaVazia()
    {
        // Act
        var dto = new SolicitarRelatoriosRequestDto();

        // Assert
        dto.TiposRelatorio.ShouldNotBeNull();
        dto.TiposRelatorio.ShouldBeEmpty();
    }

    #endregion
}
