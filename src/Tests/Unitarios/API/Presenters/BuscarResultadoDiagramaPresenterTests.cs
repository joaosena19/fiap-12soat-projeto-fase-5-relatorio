using API.Presenters;
using Application.ResultadoDiagrama.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API.Presenters;

public class BuscarResultadoDiagramaPresenterTests
{
    private readonly BuscarResultadoDiagramaPresenter _presenter = new();

    [Fact(DisplayName = "ApresentarSucesso deve mapear corretamente o aggregate")]
    [Trait("API", "BuscarResultadoDiagramaPresenter")]
    public void ApresentarSucesso_DeveMapearCorretamente_OAggregate()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder()
            .ComAnaliseDiagramaId(analiseDiagramaId)
            .Analisado()
            .ComRelatorioConcluido(TipoRelatorioEnum.Json)
            .Build();

        // Act
        _presenter.ApresentarSucesso(resultadoDiagrama);
        var actionResult = _presenter.ObterResultado();

        // Assert
        _presenter.FoiSucesso.ShouldBeTrue();
        var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        var dto = okResult.Value.ShouldBeOfType<RetornoResultadoDiagramaDto>();
        dto.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        dto.Relatorios.ShouldNotBeEmpty();
        dto.Erros.ShouldBeEmpty();
    }

    [Fact(DisplayName = "ApresentarSucesso deve mapear erros do aggregate")]
    [Trait("API", "BuscarResultadoDiagramaPresenter")]
    public void ApresentarSucesso_DeveMapearErros_DoAggregate()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder()
            .ComFalhaProcessamento()
            .Build();

        // Act
        _presenter.ApresentarSucesso(resultadoDiagrama);
        var actionResult = _presenter.ObterResultado();

        // Assert
        var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        var dto = okResult.Value.ShouldBeOfType<RetornoResultadoDiagramaDto>();
        dto.Erros.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "ApresentarSucesso deve mapear relatórios com conteúdos")]
    [Trait("API", "BuscarResultadoDiagramaPresenter")]
    public void ApresentarSucesso_DeveMapearRelatorios_ComConteudos()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder()
            .Analisado()
            .ComRelatorioConcluido(TipoRelatorioEnum.Json)
            .ComRelatorioConcluido(TipoRelatorioEnum.Markdown)
            .Build();

        // Act
        _presenter.ApresentarSucesso(resultadoDiagrama);
        var actionResult = _presenter.ObterResultado();

        // Assert
        var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        var dto = okResult.Value.ShouldBeOfType<RetornoResultadoDiagramaDto>();
        var relatoriosConcluidos = dto.Relatorios.Where(r => r.Status == StatusRelatorioEnum.Concluido).ToList();
        relatoriosConcluidos.Count.ShouldBe(2);
        relatoriosConcluidos.ShouldAllBe(r => r.Conteudos.Count > 0);
    }
}
