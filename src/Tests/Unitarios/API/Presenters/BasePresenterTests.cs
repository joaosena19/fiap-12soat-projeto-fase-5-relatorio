using API.Presenters;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API.Presenters;

public class BasePresenterTests
{
    private readonly BuscarResultadoDiagramaPresenter _presenter = new();

    [Fact(DisplayName = "ApresentarErro deve definir status code correto")]
    [Trait("API", "BasePresenter")]
    public void ApresentarErro_DeveDefinirStatusCodeCorreto()
    {
        // Act
        _presenter.ApresentarErro("recurso não encontrado", ErrorType.ResourceNotFound);
        var resultado = _presenter.ObterResultado();

        // Assert
        var objectResult = resultado.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(404);
    }

    [Fact(DisplayName = "ObterResultado deve retornar 500 quando nada foi definido")]
    [Trait("API", "BasePresenter")]
    public void ObterResultado_DeveRetornar500_QuandoNadaFoiDefinido()
    {
        // Act
        var resultado = _presenter.ObterResultado();

        // Assert
        var statusResult = resultado.ShouldBeOfType<StatusCodeResult>();
        statusResult.StatusCode.ShouldBe(500);
    }

    [Fact(DisplayName = "FoiSucesso deve ser false após apresentar erro")]
    [Trait("API", "BasePresenter")]
    public void FoiSucesso_DeveSerFalse_AposApresentarErro()
    {
        // Act
        _presenter.ApresentarErro("erro", ErrorType.InvalidInput);

        // Assert
        _presenter.FoiSucesso.ShouldBeFalse();
    }

    [Fact(DisplayName = "FoiSucesso deve ser true após apresentar sucesso")]
    [Trait("API", "BasePresenter")]
    public void FoiSucesso_DeveSerTrue_AposApresentarSucesso()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Build();

        // Act
        _presenter.ApresentarSucesso(resultadoDiagrama);

        // Assert
        _presenter.FoiSucesso.ShouldBeTrue();
    }
}
