using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Application.Extensions;
using Domain.ResultadoDiagrama.Enums;
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
        _logger = loggerFactory.CriarAppLogger<RelatorioMessagePublisher>();
    }

    public async Task PublicarSolicitacaoGeracaoAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio)
    {
        var mensagem = new SolicitarGeracaoRelatoriosDto
        {
            CorrelationId = _correlationIdAccessor.GetCorrelationId(),
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = tiposRelatorio.ToList()
        };

        _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).ComPropriedade(LogNomesPropriedades.QuantidadeTipos, tiposRelatorio.Count).LogInformation($"Publicando solicitação de geração de relatórios para {{{LogNomesPropriedades.AnaliseDiagramaId}}} com {{{LogNomesPropriedades.QuantidadeTipos}}} tipo(s)", analiseDiagramaId, tiposRelatorio.Count);

        try
        {
            await _publishEndpoint.Publish(mensagem);
        }
        catch (Exception ex)
        {
            _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogError(ex, $"Falha ao publicar solicitação de geração de relatórios para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);
            throw;
        }
    }
}