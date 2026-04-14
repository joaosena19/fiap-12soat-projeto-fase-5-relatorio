using Application.Contracts.Messaging.Dtos;
using Application.Extensions;
using Domain.ResultadoDiagrama.Enums;
using Infrastructure.Database;
using Infrastructure.Monitoramento;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumer MassTransit que consome mensagens de processamento com erro e registra a falha.
/// </summary>
public class ProcessamentoDiagramaErroConsumer : IConsumer<ProcessamentoDiagramaErroDto>
{
    private readonly AppDbContext _context;
    private readonly ILoggerFactory _loggerFactory;

    public ProcessamentoDiagramaErroConsumer(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<ProcessamentoDiagramaErroDto> context)
    {
        var mensagem = context.Message;
        var logger = _loggerFactory.CriarAppLogger<ProcessamentoDiagramaErroConsumer>();

        try
        {
            var gateway = new ResultadoDiagramaRepository(_context);
            var metrics = new NewRelicMetricsService();
            var messageId = context.MessageId?.ToString() ?? LogNomesValores.Desconhecido;

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).LogInformation($"Recebida mensagem de erro de processamento para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.MessageId}}}", mensagem.AnaliseDiagramaId, messageId);

            var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

            if (resultadoDiagrama == null)
            {
                logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogWarning($"Resultado de diagrama não encontrado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}, ignorando", mensagem.AnaliseDiagramaId);
                return;
            }

            var parsedOrigem = Enum.TryParse<OrigemErroEnum>(mensagem.OrigemErro, true, out var origem) ? origem : OrigemErroEnum.Desconhecido;

            if (mensagem.Rejeitado)
                resultadoDiagrama.RegistrarRejeicao(mensagem.Motivo, parsedOrigem, mensagem.TentativasRealizadas);
            else
                resultadoDiagrama.RegistrarFalhaProcessamento(mensagem.Motivo, parsedOrigem, mensagem.TentativasRealizadas);

            await gateway.SalvarAsync(resultadoDiagrama);

            if (mensagem.Rejeitado)
                metrics.RegistrarRejeicaoProcessamentoRecebida(mensagem.AnaliseDiagramaId, mensagem.Motivo, mensagem.OrigemErro, mensagem.TentativasRealizadas);
            else
                metrics.RegistrarFalhaProcessamentoRecebida(mensagem.AnaliseDiagramaId, mensagem.Motivo, mensagem.OrigemErro, mensagem.TentativasRealizadas);

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogWarning($"Falha registrada para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {LogNomesPropriedades.Motivo}: {{{LogNomesPropriedades.Motivo}}}. {LogNomesPropriedades.Tentativas}: {{{LogNomesPropriedades.Tentativas}}}", mensagem.AnaliseDiagramaId, mensagem.Motivo, mensagem.TentativasRealizadas);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de erro de processamento");
            throw;
        }
    }
}
