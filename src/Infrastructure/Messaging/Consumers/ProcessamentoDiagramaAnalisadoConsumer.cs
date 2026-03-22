using Application.Constants;
using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Extensions;
using Domain.AnaliseDiagrama.Entities;
using Infrastructure.Database;
using Infrastructure.Monitoramento;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging;

/// <summary>
/// Consumer MassTransit que consome mensagens de processamento analisado e registra a análise.
/// </summary>
public class ProcessamentoDiagramaAnalisadoConsumer : IConsumer<ProcessamentoDiagramaAnalisadoDto>
{
    private readonly AppDbContext _context;
    private readonly IRelatorioMessagePublisher _messagePublisher;
    private readonly ILoggerFactory _loggerFactory;

    public ProcessamentoDiagramaAnalisadoConsumer(AppDbContext context, IRelatorioMessagePublisher messagePublisher, ILoggerFactory loggerFactory)
    {
        _context = context;
        _messagePublisher = messagePublisher;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<ProcessamentoDiagramaAnalisadoDto> context)
    {
        var mensagem = context.Message;
        var logger = new LoggerAdapter<ProcessamentoDiagramaAnalisadoConsumer>(_loggerFactory.CreateLogger<ProcessamentoDiagramaAnalisadoConsumer>());

        try
        {
            var gateway = new ResultadoDiagramaRepository(_context);
            var metrics = new NewRelicMetricsService();
            var messageId = context.MessageId?.ToString() ?? "desconhecido";

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).LogInformation($"Recebida mensagem de processamento analisado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.MessageId}}}", mensagem.AnaliseDiagramaId, messageId);

            var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

            if (resultadoDiagrama == null)
            {
                logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogWarning($"Resultado de diagrama não encontrado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}, ignorando", mensagem.AnaliseDiagramaId);
                return;
            }

            var analiseResultado = AnaliseResultado.Criar(mensagem.DescricaoAnalise, mensagem.ComponentesIdentificados, mensagem.RiscosArquiteturais, mensagem.RecomendacoesBasicas);

            resultadoDiagrama.RegistrarAnalise(analiseResultado);

            await gateway.SalvarAsync(resultadoDiagrama);

            await _messagePublisher.PublicarSolicitacaoGeracaoAsync(mensagem.AnaliseDiagramaId, TiposRelatorioPadrao.Tipos);

            metrics.RegistrarAnaliseConcluida(mensagem.AnaliseDiagramaId);

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogInformation($"Análise registrada com sucesso para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de processamento analisado");
            throw;
        }
    }
}
