using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento;

public class CorrelationContextTests
{
    [Fact(DisplayName = "Current deve retornar null quando nenhum ID foi definido")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Current_DeveRetornarNull_QuandoNenhumIdFoiDefinido()
    {
        // Act
        var resultado = CorrelationContext.Current;

        // Assert
        resultado.ShouldBeNull();
    }

    [Fact(DisplayName = "Push deve definir o valor do Current")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Push_DeveDefinirValorDoCurrent_QuandoChamado()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();

        // Act
        using (CorrelationContext.Push(correlationId))
        {
            // Assert
            CorrelationContext.Current.ShouldBe(correlationId);
        }
    }

    [Fact(DisplayName = "Dispose deve restaurar o valor anterior")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Dispose_DeveRestaurarValorAnterior_QuandoScopeEncerrado()
    {
        // Arrange
        var primeiroId = Guid.NewGuid().ToString();
        var segundoId = Guid.NewGuid().ToString();

        using (CorrelationContext.Push(primeiroId))
        {
            // Act
            using (CorrelationContext.Push(segundoId))
                CorrelationContext.Current.ShouldBe(segundoId);

            // Assert
            CorrelationContext.Current.ShouldBe(primeiroId);
        }
    }

    [Fact(DisplayName = "Push aninhado deve funcionar corretamente com múltiplos níveis")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Push_DeveFuncionarCorretamente_QuandoAninhadoComMultiplosNiveis()
    {
        // Arrange
        var idNivel1 = Guid.NewGuid().ToString();
        var idNivel2 = Guid.NewGuid().ToString();
        var idNivel3 = Guid.NewGuid().ToString();

        // Act & Assert
        using (CorrelationContext.Push(idNivel1))
        {
            CorrelationContext.Current.ShouldBe(idNivel1);

            using (CorrelationContext.Push(idNivel2))
            {
                CorrelationContext.Current.ShouldBe(idNivel2);

                using (CorrelationContext.Push(idNivel3))
                    CorrelationContext.Current.ShouldBe(idNivel3);

                CorrelationContext.Current.ShouldBe(idNivel2);
            }

            CorrelationContext.Current.ShouldBe(idNivel1);
        }

        CorrelationContext.Current.ShouldBeNull();
    }

    [Fact(DisplayName = "Dispose duplo deve ser seguro")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Dispose_DeveSerSeguro_QuandoChamadoDuasVezes()
    {
        // Arrange
        var primeiroId = Guid.NewGuid().ToString();
        var segundoId = Guid.NewGuid().ToString();

        using (CorrelationContext.Push(primeiroId))
        {
            var scope = CorrelationContext.Push(segundoId);

            // Act
            scope.Dispose();
            scope.Dispose();

            // Assert
            CorrelationContext.Current.ShouldBe(primeiroId);
        }
    }
}
