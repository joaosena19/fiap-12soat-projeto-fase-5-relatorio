using API.Presenters;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API.Presenters;

public class SolicitarGeracaoRelatoriosPresenterTests
{
    private readonly SolicitarGeracaoRelatoriosPresenter _presenter = new();

    [Fact(DisplayName = "Deve retornar 200 quando todos os relatórios estão concluídos")]
    [Trait("API", "SolicitarGeracaoRelatoriosPresenter")]
    public void ApresentarSucesso_DeveRetornar200_QuandoTodosRelatoriosConcluidos()
    {
        // Arrange
        var resultado = new ResultadoSolicitacaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            Relatorios = new List<ItemResultadoSolicitacaoRelatorioDto>
            {
                new() { Tipo = TipoRelatorioEnum.Json, Resultado = ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido },
                new() { Tipo = TipoRelatorioEnum.Markdown, Resultado = ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido }
            }
        };

        // Act
        _presenter.ApresentarSucesso(resultado);
        var actionResult = _presenter.ObterResultado();

        // Assert
        var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(200);
        _presenter.FoiSucesso.ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve retornar 202 quando todos os relatórios estão aceitos para geração")]
    [Trait("API", "SolicitarGeracaoRelatoriosPresenter")]
    public void ApresentarSucesso_DeveRetornar202_QuandoTodosRelatoriosAceitosParaGeracao()
    {
        // Arrange
        var resultado = new ResultadoSolicitacaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            Relatorios = new List<ItemResultadoSolicitacaoRelatorioDto>
            {
                new() { Tipo = TipoRelatorioEnum.Json, Resultado = ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao },
                new() { Tipo = TipoRelatorioEnum.Markdown, Resultado = ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao }
            }
        };

        // Act
        _presenter.ApresentarSucesso(resultado);
        var actionResult = _presenter.ObterResultado();

        // Assert
        var objectResult = actionResult.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(202);
    }

    [Fact(DisplayName = "Deve retornar 207 quando há status mistos")]
    [Trait("API", "SolicitarGeracaoRelatoriosPresenter")]
    public void ApresentarSucesso_DeveRetornar207_QuandoStatusMistos()
    {
        // Arrange
        var resultado = new ResultadoSolicitacaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            Relatorios = new List<ItemResultadoSolicitacaoRelatorioDto>
            {
                new() { Tipo = TipoRelatorioEnum.Json, Resultado = ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido },
                new() { Tipo = TipoRelatorioEnum.Markdown, Resultado = ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao }
            }
        };

        // Act
        _presenter.ApresentarSucesso(resultado);
        var actionResult = _presenter.ObterResultado();

        // Assert
        var objectResult = actionResult.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(207);
    }

    [Fact(DisplayName = "Deve retornar 202 quando todos são JaEmAndamento")]
    [Trait("API", "SolicitarGeracaoRelatoriosPresenter")]
    public void ApresentarSucesso_DeveRetornar202_QuandoTodosJaEmAndamento()
    {
        // Arrange
        var resultado = new ResultadoSolicitacaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            Relatorios = new List<ItemResultadoSolicitacaoRelatorioDto>
            {
                new() { Tipo = TipoRelatorioEnum.Json, Resultado = ResultadoSolicitacaoGeracaoRelatorioEnum.JaEmAndamento }
            }
        };

        // Act
        _presenter.ApresentarSucesso(resultado);
        var actionResult = _presenter.ObterResultado();

        // Assert
        var objectResult = actionResult.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(202);
    }
}
