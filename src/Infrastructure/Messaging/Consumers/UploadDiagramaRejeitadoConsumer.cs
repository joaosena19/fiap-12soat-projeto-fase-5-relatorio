using Application.Contracts.Messaging.Dtos;
using Application.Extensions;
using Infrastructure.Database;
using Infrastructure.Monitoramento;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging.Consumers;

public class UploadDiagramaRejeitadoConsumer : IConsumer<UploadDiagramaRejeitadoDto>
{
    private readonly AppDbContext _context;
    private readonly ILoggerFactory _loggerFactory;

    public UploadDiagramaRejeitadoConsumer(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<UploadDiagramaRejeitadoDto> context)
    {
        var mensagem = context.Message;
        var logger = _loggerFactory.CriarAppLogger<UploadDiagramaRejeitadoConsumer>();

        try
        {
            var gateway = new ResultadoDiagramaRepository(_context);
            var metrics = new NewRelicMetricsService();
            var messageId = context.MessageId?.ToString() ?? LogNomesValores.Desconhecido;

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).ComPropriedade(LogNomesPropriedades.Motivo, mensagem.MotivoRejeicao).LogInformation($"Recebida mensagem de upload rejeitado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.MessageId}}}", mensagem.AnaliseDiagramaId, messageId);

            var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId) ?? Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama.Criar(mensagem.AnaliseDiagramaId);

            resultadoDiagrama.RegistrarFalhaProcessamento(mensagem.MotivoRejeicao);

            await gateway.SalvarAsync(resultadoDiagrama);

            metrics.RegistrarAnaliseComFalha(mensagem.AnaliseDiagramaId, mensagem.MotivoRejeicao);

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.Motivo, mensagem.MotivoRejeicao).LogInformation($"Rejeição de upload registrada para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {LogNomesPropriedades.Motivo}: {{{LogNomesPropriedades.Motivo}}}", mensagem.AnaliseDiagramaId, mensagem.MotivoRejeicao);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de upload rejeitado");
            throw;
        }
    }
}