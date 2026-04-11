using Application.Contracts.Messaging.Dtos;
using Application.Extensions;
using Infrastructure.Database;
using Infrastructure.Monitoramento;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumer MassTransit que consome mensagens de processamento iniciado e atualiza o resultado.
/// </summary>
public class ProcessamentoDiagramaIniciadoConsumer : IConsumer<ProcessamentoDiagramaIniciadoDto>
{
    private readonly AppDbContext _context;
    private readonly ILoggerFactory _loggerFactory;

    public ProcessamentoDiagramaIniciadoConsumer(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<ProcessamentoDiagramaIniciadoDto> context)
    {
        var mensagem = context.Message;
        var logger = _loggerFactory.CriarAppLogger<ProcessamentoDiagramaIniciadoConsumer>();

        try
        {
            var gateway = new ResultadoDiagramaRepository(_context);
            var metrics = new NewRelicMetricsService();
            var messageId = context.MessageId?.ToString() ?? LogNomesValores.Desconhecido;

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).LogInformation($"Recebida mensagem de processamento iniciado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.MessageId}}}", mensagem.AnaliseDiagramaId, messageId);

            var resultadoExistente = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

            var resultadoDiagrama = resultadoExistente ?? Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama.Criar(mensagem.AnaliseDiagramaId);
            resultadoDiagrama.MarcarEmProcessamento();

            await gateway.SalvarAsync(resultadoDiagrama);

            metrics.RegistrarAnaliseRecebida(mensagem.AnaliseDiagramaId, mensagem.Extensao);

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogInformation($"Resultado de diagrama atualizado para EmProcessamento em {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de processamento iniciado");
            throw;
        }
    }
}
