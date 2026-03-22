using Application.Contracts.Gateways;
using Application.Contracts.Messaging.Dtos;
using Application.Extensions;
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

        try
        {
            var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();
            var messageId = context.MessageId?.ToString() ?? "desconhecido";

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).LogInformation($"Recebida mensagem de upload concluído para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.MessageId}}}", mensagem.AnaliseDiagramaId, messageId);

            var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);
            if (resultadoDiagrama != null)
            {
                logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogDebug("Resultado de diagrama já existe, ignorando mensagem de upload concluído");
                return;
            }

            resultadoDiagrama = Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama.Criar(mensagem.AnaliseDiagramaId);
            await gateway.SalvarAsync(resultadoDiagrama);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de upload concluído");
            throw;
        }
    }
}