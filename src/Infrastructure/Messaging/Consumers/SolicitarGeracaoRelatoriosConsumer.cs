using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Relatorios;
using Application.Extensions;
using Application.ResultadoDiagrama.UseCases;
using Infrastructure.Database;
using Infrastructure.Monitoramento;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging.Consumers;

public class SolicitarGeracaoRelatoriosConsumer : IConsumer<SolicitarGeracaoRelatoriosDto>
{
    private readonly AppDbContext _context;
    private readonly IRelatorioStrategyResolver _strategyResolver;
    private readonly ILoggerFactory _loggerFactory;

    public SolicitarGeracaoRelatoriosConsumer(AppDbContext context, IRelatorioStrategyResolver strategyResolver, ILoggerFactory loggerFactory)
    {
        _context = context;
        _strategyResolver = strategyResolver;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<SolicitarGeracaoRelatoriosDto> context)
    {
        var mensagem = context.Message;
        var logger = _loggerFactory.CriarAppLogger<SolicitarGeracaoRelatoriosConsumer>();

        try
        {
            var gateway = new ResultadoDiagramaRepository(_context);
            var metrics = new NewRelicMetricsService();
            var useCase = new GerarRelatorioUseCase();
            var messageId = context.MessageId?.ToString() ?? LogNomesValores.Desconhecido;

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).ComPropriedade(LogNomesPropriedades.QuantidadeTipos, mensagem.TiposRelatorio.Count).LogInformation($"Recebida solicitação assíncrona de geração de relatórios com {{{LogNomesPropriedades.QuantidadeTipos}}} tipo(s). {{{LogNomesPropriedades.MessageId}}}", mensagem.TiposRelatorio.Count, messageId);

            foreach (var tipoRelatorio in mensagem.TiposRelatorio.Distinct())
                await useCase.ExecutarAsync(mensagem.AnaliseDiagramaId, tipoRelatorio, gateway, _strategyResolver, metrics, logger);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir solicitação de geração de relatórios");
            throw;
        }
    }
}