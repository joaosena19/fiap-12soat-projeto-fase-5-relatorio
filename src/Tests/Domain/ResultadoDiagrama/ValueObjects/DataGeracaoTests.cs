using DataGeracaoRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.DataGeracao;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class DataGeracaoTests
{
    [Fact(DisplayName = "Deve criar data de geração com valor válido")]
    [Trait("ValueObject", "DataGeracao")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = DateTimeOffset.UtcNow;

        // Act
        var dataGeracao = new DataGeracaoRelatorio(valor);

        // Assert
        dataGeracao.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando data de geração é default")]
    [Trait("ValueObject", "DataGeracao")]
    public void Constructor_DeveLancarExcecao_QuandoValorDefault()
    {
        // Arrange
        Action acao = () => _ = new DataGeracaoRelatorio(default);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Data de geração inválida");
    }
}