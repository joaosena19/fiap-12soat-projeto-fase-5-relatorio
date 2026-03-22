using Infrastructure.Monitoramento.Correlation;
using Serilog.Context;

namespace API.Middleware;

/// <summary>
/// Middleware que garante a propagação do header X-Correlation-ID em todas as requisições.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headerValue = context.Request.Headers[CorrelationConstants.HeaderName].FirstOrDefault();

        string correlationId;
        if (string.IsNullOrWhiteSpace(headerValue))
            correlationId = Guid.NewGuid().ToString();
        else
            correlationId = headerValue;

        context.Request.Headers.Remove(CorrelationConstants.HeaderName);
        context.Request.Headers.Append(CorrelationConstants.HeaderName, correlationId);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationConstants.HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (CorrelationContext.Push(correlationId))
        using (LogContext.PushProperty(CorrelationConstants.LogPropertyName, correlationId))
            await _next(context);
    }
}
