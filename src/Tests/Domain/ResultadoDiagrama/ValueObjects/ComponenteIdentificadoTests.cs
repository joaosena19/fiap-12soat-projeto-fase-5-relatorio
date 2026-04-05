using Domain.ResultadoDiagrama.ValueObjects.AnaliseResultado;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class ComponenteIdentificadoTests
{
    [Fact(DisplayName = "Deve criar componente identificado quando valor é válido")]
    [Trait("ValueObject", "ComponenteIdentificado")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var componenteIdentificado = new ComponenteIdentificado(" API Gateway ");

        // Assert
        componenteIdentificado.Valor.ShouldBe("API Gateway");
    }

    [Fact(DisplayName = "Deve lançar exceção quando componente identificado é vazio")]
    [Trait("ValueObject", "ComponenteIdentificado")]
    public void Constructor_DeveLancarExcecao_QuandoValorVazio()
    {
        // Arrange
        Action acao = () => _ = new ComponenteIdentificado(" ");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Componente identificado");
    }
}