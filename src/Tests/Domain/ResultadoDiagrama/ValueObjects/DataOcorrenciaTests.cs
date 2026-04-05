using DataOcorrenciaErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.DataOcorrencia;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class DataOcorrenciaTests
{
    [Fact(DisplayName = "Deve criar data de ocorrência com valor válido")]
    [Trait("ValueObject", "DataOcorrencia")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = DateTimeOffset.UtcNow;

        // Act
        var dataOcorrenciaErro = new DataOcorrenciaErro(valor);

        // Assert
        dataOcorrenciaErro.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando data de ocorrência é default")]
    [Trait("ValueObject", "DataOcorrencia")]
    public void Constructor_DeveLancarExcecao_QuandoValorDefault()
    {
        // Arrange
        Action acao = () => _ = new DataOcorrenciaErro(default);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Data de ocorrência inválida");
    }
}