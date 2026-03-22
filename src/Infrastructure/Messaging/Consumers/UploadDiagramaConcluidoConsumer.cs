using Application.Contracts.Gateways;
using Application.Contracts.Messaging.Dtos;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging;

public class UploadDiagramaConcluidoConsumer : IConsumer<UploadDiagramaConcluidoDto>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public UploadDiagramaConcluidoConsumer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<UploadDiagramaConcluidoDto> context)
    {
        var mensagem = context.Message;
        var logger = new LoggerAdapter<UploadDiagramaConcluidoConsumer>(_loggerFactory.CreateLogger<UploadDiagramaConcluidoConsumer>());
        var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();

        logger.LogInformation($"Recebida mensagem de upload concluído para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);

        var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);
        if (resultadoDiagrama != null)
            return;

        resultadoDiagrama = Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama.Criar(mensagem.AnaliseDiagramaId);
        await gateway.SalvarAsync(resultadoDiagrama);
    }
}