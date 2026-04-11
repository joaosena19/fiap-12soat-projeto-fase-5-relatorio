using Domain.ResultadoDiagrama.ValueObjects.AnaliseResultado;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class RecomendacaoBasicaTests
{
    [Fact(DisplayName = "Deve criar recomendação básica quando valor é válido")]
    [Trait("ValueObject", "RecomendacaoBasica")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var recomendacaoBasica = new RecomendacaoBasica(" Separar bounded contexts ");

        // Assert
        recomendacaoBasica.Valor.ShouldBe("Separar bounded contexts");
    }

    [Fact(DisplayName = "Deve lançar exceção quando recomendação básica é vazia")]
    [Trait("ValueObject", "RecomendacaoBasica")]
    public void Constructor_DeveLancarExcecao_QuandoValorVazio()
    {
        // Arrange
        Action acao = () => _ = new RecomendacaoBasica(" ");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Recomendação básica");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "RecomendacaoBasica")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (RecomendacaoBasica)Activator.CreateInstance(typeof(RecomendacaoBasica), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}