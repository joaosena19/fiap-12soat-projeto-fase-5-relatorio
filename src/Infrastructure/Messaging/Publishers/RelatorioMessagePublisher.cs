using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Application.Extensions;
using Domain.AnaliseDiagrama.Enums;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging.Publishers;

public class RelatorioMessagePublisher : IRelatorioMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    private readonly IAppLogger _logger;

    public RelatorioMessagePublisher(IPublishEndpoint publishEndpoint, ICorrelationIdAccessor correlationIdAccessor, ILoggerFactory loggerFactory)
    {
        _publishEndpoint = publishEndpoint;
        _correlationIdAccessor = correlationIdAccessor;
        _logger = new LoggerAdapter<RelatorioMessagePublisher>(loggerFactory.CreateLogger<RelatorioMessagePublisher>());
    }

    public async Task PublicarSolicitacaoGeracaoAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio)
    {
        _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).ComPropriedade(LogNomesPropriedades.QuantidadeTipos, tiposRelatorio.Count).LogInformation($"Publicando solicitação de geração de relatórios para {{{LogNomesPropriedades.AnaliseDiagramaId}}} com {{{LogNomesPropriedades.QuantidadeTipos}}} tipo(s)", analiseDiagramaId, tiposRelatorio.Count);

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