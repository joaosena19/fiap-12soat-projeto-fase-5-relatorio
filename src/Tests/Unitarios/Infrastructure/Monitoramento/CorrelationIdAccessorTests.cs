using Infrastructure.Monitoramento;
using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento;

public class CorrelationIdAccessorTests
{
    private readonly CorrelationIdAccessor _accessor = new();

    [Fact(DisplayName = "Deve retornar correlation ID existente quando presente no contexto")]
    [Trait("Infrastructure", "CorrelationIdAccessor")]
    public void GetCorrelationId_DeveRetornarIdExistente_QuandoPresenteNoContexto()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            var resultado = _accessor.GetCorrelationId();

            // Assert
            resultado.ShouldBe(correlationId);
        }
    }

    [Fact(DisplayName = "Deve gerar novo GUID quando contexto está vazio")]
    [Trait("Infrastructure", "CorrelationIdAccessor")]
    public void GetCorrelationId_DeveGerarNovoGuid_QuandoContextoVazio()
    {
        // Act
        var resultado = _accessor.GetCorrelationId();

        // Assert
        resultado.ShouldNotBeNullOrWhiteSpace();
        Guid.TryParse(resultado, out _).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve gerar GUIDs diferentes a cada chamada quando contexto está vazio")]
    [Trait("Infrastructure", "CorrelationIdAccessor")]
    public void GetCorrelationId_DeveGerarGuidsDiferentes_QuandoContextoVazio()
    {
        // Act
        var resultado1 = _accessor.GetCorrelationId();
        var resultado2 = _accessor.GetCorrelationId();

        // Assert
        resultado1.ShouldNotBe(resultado2);
    }
}
