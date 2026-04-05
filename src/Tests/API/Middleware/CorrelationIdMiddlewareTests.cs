using API.Middleware;
using Infrastructure.Monitoramento.Correlation;
using Microsoft.AspNetCore.Http;

namespace Tests.API.Middleware;

public class CorrelationIdMiddlewareTests
{
    [Fact(DisplayName = "Deve extrair correlation ID do header da requisição")]
    [Trait("API", "CorrelationIdMiddleware")]
    public async Task InvokeAsync_DeveExtrairCorrelationId_DoHeader()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var contexto = new DefaultHttpContext();
        contexto.Request.Headers[CorrelationConstants.HeaderName] = correlationId;
        string? correlationIdCapturado = null;

        var middleware = new CorrelationIdMiddleware(ctx =>
        {
            correlationIdCapturado = CorrelationContext.Current;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(contexto);

        // Assert
        correlationIdCapturado.ShouldBe(correlationId);
    }

    [Fact(DisplayName = "Deve gerar correlation ID quando header não está presente")]
    [Trait("API", "CorrelationIdMiddleware")]
    public async Task InvokeAsync_DeveGerarCorrelationId_QuandoHeaderNaoPresente()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        string? correlationIdCapturado = null;

        var middleware = new CorrelationIdMiddleware(ctx =>
        {
            correlationIdCapturado = CorrelationContext.Current;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(contexto);

        // Assert
        correlationIdCapturado.ShouldNotBeNullOrWhiteSpace();
        Guid.TryParse(correlationIdCapturado, out _).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve propagar correlation ID recebido no header da requisição")]
    [Trait("API", "CorrelationIdMiddleware")]
    public async Task InvokeAsync_DevePropagarCorrelationIdRecebido_NoHeaderDaRequisicao()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var contexto = new DefaultHttpContext();
        contexto.Request.Headers[CorrelationConstants.HeaderName] = correlationId;

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(contexto);
        var headerRequisicao = contexto.Request.Headers[CorrelationConstants.HeaderName].ToString();

        // Assert
        headerRequisicao.ShouldBe(correlationId);
    }

    [Fact(DisplayName = "Deve definir correlation ID no header da requisição quando gerado")]
    [Trait("API", "CorrelationIdMiddleware")]
    public async Task InvokeAsync_DeveDefinirCorrelationId_NoHeaderDaRequisicao()
    {
        // Arrange
        var contexto = new DefaultHttpContext();

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(contexto);

        // Assert
        var headerRequisicao = contexto.Request.Headers[CorrelationConstants.HeaderName].ToString();
        headerRequisicao.ShouldNotBeNullOrWhiteSpace();
    }
}
