using StatusAnalise = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.Status;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class StatusAnaliseTests
{
    [Theory(DisplayName = "Deve criar status de análise com valor válido")]
    [InlineData(StatusAnaliseEnum.Recebido)]
    [InlineData(StatusAnaliseEnum.EmProcessamento)]
    [InlineData(StatusAnaliseEnum.Analisado)]
    [InlineData(StatusAnaliseEnum.Erro)]
    [Trait("ValueObject", "StatusAnalise")]
    public void Constructor_DeveCriar_QuandoValorValido(StatusAnaliseEnum valor)
    {
        // Act
        var status = new StatusAnalise(valor);

        // Assert
        status.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando status de análise é inválido")]
    [Trait("ValueObject", "StatusAnalise")]
    public void Constructor_DeveLancarExcecao_QuandoValorInvalido()
    {
        // Arrange
        Action acao = () => _ = new StatusAnalise((StatusAnaliseEnum)999);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Status de análise");
    }
}