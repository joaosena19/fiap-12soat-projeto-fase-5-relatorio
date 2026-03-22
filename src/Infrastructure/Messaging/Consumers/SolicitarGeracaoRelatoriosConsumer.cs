using Application.Contracts.Gateways;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Relatorios;
using Application.ResultadoDiagrama.UseCases;
using Domain.AnaliseDiagrama.Enums;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();
        var strategyResolver = _serviceProvider.GetRequiredService<IRelatorioStrategyResolver>();
        var useCase = new GerarRelatorioUseCase();

        foreach (var tipoRelatorio in mensagem.TiposRelatorio.Distinct())
            await useCase.ExecutarAsync(mensagem.AnaliseDiagramaId, tipoRelatorio, gateway, strategyResolver, logger);
    }
}