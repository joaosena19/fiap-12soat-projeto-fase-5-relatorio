using Application.Contracts.Gateways;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
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
        var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();
        var metrics = _serviceProvider.GetRequiredService<IMetricsService>();

        logger.LogInformation($"Recebida mensagem de erro de processamento para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);

        var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

        if (resultadoDiagrama == null)
        {
            logger.LogWarning($"Resultado de diagrama não encontrado para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}, ignorando", mensagem.AnaliseDiagramaId);
            return;
        }

        resultadoDiagrama.RegistrarFalhaProcessamento(mensagem.Motivo);

        await gateway.SalvarAsync(resultadoDiagrama);

        metrics.RegistrarAnaliseComFalha(mensagem.AnaliseDiagramaId, mensagem.Motivo);

        logger.LogWarning($"Falha registrada para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {LogNomesPropriedades.Motivo}: {{{LogNomesPropriedades.Motivo}}}. {LogNomesPropriedades.Tentativas}: {{{LogNomesPropriedades.Tentativas}}}", mensagem.AnaliseDiagramaId, mensagem.Motivo, mensagem.TentativasRealizadas);
    }
}
