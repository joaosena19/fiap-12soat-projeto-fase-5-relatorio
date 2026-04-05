using DataCriacaoResultadoDiagrama = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataCriacao;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class DataCriacaoTests
{
    [Fact(DisplayName = "Deve criar data de criação com valor válido")]
    [Trait("ValueObject", "DataCriacao")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = DateTimeOffset.UtcNow;

        // Act
        var dataCriacao = new DataCriacaoResultadoDiagrama(valor);

        // Assert
        dataCriacao.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando data de criação é default")]
    [Trait("ValueObject", "DataCriacao")]
    public void Constructor_DeveLancarExcecao_QuandoValorDefault()
    {
        // Arrange
        Action acao = () => _ = new DataCriacaoResultadoDiagrama(default);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Data de criação inválida");
    }
}