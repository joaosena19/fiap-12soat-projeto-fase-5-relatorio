using Domain.ResultadoDiagrama.ValueObjects.AnaliseResultado;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class DescricaoAnaliseTests
{
    [Fact(DisplayName = "Deve criar descrição quando valor é válido")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var descricaoAnalise = new DescricaoAnalise(" Descrição válida ");

        // Assert
        descricaoAnalise.Valor.ShouldBe("Descrição válida");
    }

    [Fact(DisplayName = "Deve lançar exceção quando descrição é vazia")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Constructor_DeveLancarExcecao_QuandoValorVazio()
    {
        // Arrange
        Action acao = () => _ = new DescricaoAnalise(" ");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Descrição da análise");
    }
}