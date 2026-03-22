using Application.Contracts.Gateways;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Application.Contracts.Relatorios;
using Application.Extensions;
using Application.ResultadoDiagrama.UseCases;
using Domain.AnaliseDiagrama.Enums;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging;

public class SolicitarGeracaoRelatoriosConsumer : IConsumer<SolicitarGeracaoRelatoriosDto>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public SolicitarGeracaoRelatoriosConsumer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<SolicitarGeracaoRelatoriosDto> context)
    {
        var mensagem = context.Message;
        var logger = new LoggerAdapter<SolicitarGeracaoRelatoriosConsumer>(_loggerFactory.CreateLogger<SolicitarGeracaoRelatoriosConsumer>());

        try
        {
            var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();
            var strategyResolver = _serviceProvider.GetRequiredService<IRelatorioStrategyResolver>();
            var metrics = _serviceProvider.GetRequiredService<IMetricsService>();
            var useCase = new GerarRelatorioUseCase();
            var messageId = context.MessageId?.ToString() ?? "desconhecido";

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).ComPropriedade(LogNomesPropriedades.QuantidadeTipos, mensagem.TiposRelatorio.Count).LogInformation($"Recebida solicitação assíncrona de geração de relatórios com {{{LogNomesPropriedades.QuantidadeTipos}}} tipo(s). {{{LogNomesPropriedades.MessageId}}}", mensagem.TiposRelatorio.Count, messageId);

            foreach (var tipoRelatorio in mensagem.TiposRelatorio.Distinct())
                await useCase.ExecutarAsync(mensagem.AnaliseDiagramaId, tipoRelatorio, gateway, strategyResolver, metrics, logger);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir solicitação de geração de relatórios");
            throw;
        }
    }
}