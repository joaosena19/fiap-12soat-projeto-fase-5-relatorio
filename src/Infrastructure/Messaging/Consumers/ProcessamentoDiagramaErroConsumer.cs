using Application.Contracts.Gateways;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Application.Extensions;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging;

/// <summary>
/// Consumer MassTransit que consome mensagens de processamento com erro e registra a falha.
/// </summary>
public class ProcessamentoDiagramaErroConsumer : IConsumer<ProcessamentoDiagramaErroDto>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public ProcessamentoDiagramaErroConsumer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<ProcessamentoDiagramaErroDto> context)
    {
        var mensagem = context.Message;
        var logger = new LoggerAdapter<ProcessamentoDiagramaErroConsumer>(_loggerFactory.CreateLogger<ProcessamentoDiagramaErroConsumer>());

        try
        {
            var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();
            var metrics = _serviceProvider.GetRequiredService<IMetricsService>();
            var messageId = context.MessageId?.ToString() ?? "desconhecido";

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).LogInformation($"Recebida mensagem de erro de processamento para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.MessageId}}}", mensagem.AnaliseDiagramaId, messageId);

            var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

            if (resultadoDiagrama == null)
            {
                logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogWarning($"Resultado de diagrama não encontrado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}, ignorando", mensagem.AnaliseDiagramaId);
                return;
            }

            resultadoDiagrama.RegistrarFalhaProcessamento(mensagem.Motivo);

            await gateway.SalvarAsync(resultadoDiagrama);

            metrics.RegistrarAnaliseComFalha(mensagem.AnaliseDiagramaId, mensagem.Motivo);

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogWarning($"Falha registrada para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {LogNomesPropriedades.Motivo}: {{{LogNomesPropriedades.Motivo}}}. {LogNomesPropriedades.Tentativas}: {{{LogNomesPropriedades.Tentativas}}}", mensagem.AnaliseDiagramaId, mensagem.Motivo, mensagem.TentativasRealizadas);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de erro de processamento");
            throw;
        }
    }
}
