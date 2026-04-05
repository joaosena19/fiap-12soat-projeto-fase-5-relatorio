using Domain.ResultadoDiagrama.ValueObjects.AnaliseResultado;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class RiscoArquiteturalTests
{
    [Fact(DisplayName = "Deve criar risco arquitetural quando valor é válido")]
    [Trait("ValueObject", "RiscoArquitetural")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var riscoArquitetural = new RiscoArquitetural(" Ponto único de falha ");

        // Assert
        riscoArquitetural.Valor.ShouldBe("Ponto único de falha");
    }

    [Fact(DisplayName = "Deve lançar exceção quando risco arquitetural é vazio")]
    [Trait("ValueObject", "RiscoArquitetural")]
    public void Constructor_DeveLancarExcecao_QuandoValorVazio()
    {
        // Arrange
        Action acao = () => _ = new RiscoArquitetural(" ");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Risco arquitetural");
    }
}