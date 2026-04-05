using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Filters;

public class CorrelationIdFilterHelperTests
{
    [Fact(DisplayName = "Deve usar correlation ID do contexto quando disponível")]
    [Trait("Infrastructure", "CorrelationIdFilterHelper")]
    public void AplicarCorrelationId_DeveUsarIdDoContexto_QuandoDisponivel()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var (sendContextMock, headersMock) = SendContextMockExtensions.CriarSendContext<object>();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            CorrelationIdFilterHelper.AplicarCorrelationId(sendContextMock.Object);
        }

        // Assert
        headersMock.Verify(x => x.Set(CorrelationConstants.HeaderName, correlationId), Times.Once);
        sendContextMock.VerifySet(x => x.CorrelationId = Guid.Parse(correlationId), Times.Once);
    }

    [Fact(DisplayName = "Deve gerar novo correlation ID quando contexto está vazio")]
    [Trait("Infrastructure", "CorrelationIdFilterHelper")]
    public void AplicarCorrelationId_DeveGerarNovoId_QuandoContextoVazio()
    {
        // Arrange
        var (sendContextMock, headersMock) = SendContextMockExtensions.CriarSendContext<object>();

        // Act
        CorrelationIdFilterHelper.AplicarCorrelationId(sendContextMock.Object);

        // Assert
        headersMock.Verify(x => x.Set(CorrelationConstants.HeaderName, It.IsAny<string>()), Times.Once);
    }

    [Fact(DisplayName = "Deve definir CorrelationId como Guid quando ID é GUID válido")]
    [Trait("Infrastructure", "CorrelationIdFilterHelper")]
    public void AplicarCorrelationId_DeveDefinirCorrelationIdComoGuid_QuandoIdEhGuidValido()
    {
        // Arrange
        var guidCorrelation = Guid.NewGuid();
        var (sendContextMock, _) = SendContextMockExtensions.CriarSendContext<object>();

        using (CorrelationContext.Push(guidCorrelation.ToString()))
        {
            // Act
            CorrelationIdFilterHelper.AplicarCorrelationId(sendContextMock.Object);
        }

        // Assert
        sendContextMock.VerifySet(x => x.CorrelationId = guidCorrelation, Times.Once);
    }
}
