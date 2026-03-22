using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Domain.AnaliseDiagrama.Enums;
using MassTransit;

namespace Infrastructure.Messaging.Publishers;

public class RelatorioMessagePublisher : IRelatorioMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public RelatorioMessagePublisher(IPublishEndpoint publishEndpoint, ICorrelationIdAccessor correlationIdAccessor)
    {
        _publishEndpoint = publishEndpoint;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public async Task PublicarSolicitacaoGeracaoAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio)
    {
        await _publishEndpoint.Publish(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = tiposRelatorio.ToList()
        }, context =>
        {
            if (Guid.TryParse(_correlationIdAccessor.GetCorrelationId(), out var correlationId))
                context.CorrelationId = correlationId;
        });
    }
}