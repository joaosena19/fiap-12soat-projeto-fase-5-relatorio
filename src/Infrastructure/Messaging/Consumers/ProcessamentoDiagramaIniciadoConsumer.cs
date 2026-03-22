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
/// Consumer MassTransit que consome mensagens de processamento iniciado e atualiza o resultado.
/// </summary>
public class ProcessamentoDiagramaIniciadoConsumer : IConsumer<ProcessamentoDiagramaIniciadoDto>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public ProcessamentoDiagramaIniciadoConsumer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<ProcessamentoDiagramaIniciadoDto> context)
    {
        var mensagem = context.Message;
        var logger = new LoggerAdapter<ProcessamentoDiagramaIniciadoConsumer>(_loggerFactory.CreateLogger<ProcessamentoDiagramaIniciadoConsumer>());
        var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();
        var metrics = _serviceProvider.GetRequiredService<IMetricsService>();

        logger.LogInformation($"Recebida mensagem de processamento iniciado para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);

        var resultadoExistente = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

        var resultadoDiagrama = resultadoExistente ?? Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama.Criar(mensagem.AnaliseDiagramaId);
        resultadoDiagrama.MarcarEmProcessamento();

        await gateway.SalvarAsync(resultadoDiagrama);

        metrics.RegistrarAnaliseRecebida(mensagem.AnaliseDiagramaId, mensagem.Extensao);

        logger.LogInformation($"Resultado de diagrama atualizado para EmProcessamento em {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);
    }
}
