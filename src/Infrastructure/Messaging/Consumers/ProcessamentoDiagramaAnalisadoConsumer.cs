using Application.Contracts.Gateways;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Domain.AnaliseDiagrama.Entities;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using System.Text.Json;

namespace Infrastructure.Messaging;

/// <summary>
/// Consumer MassTransit que consome mensagens de processamento analisado e registra a análise.
/// </summary>
public class ProcessamentoDiagramaAnalisadoConsumer : IConsumer<ProcessamentoDiagramaAnalisadoDto>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public ProcessamentoDiagramaAnalisadoConsumer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<ProcessamentoDiagramaAnalisadoDto> context)
    {
        var mensagem = context.Message;
        var logger = new LoggerAdapter<ProcessamentoDiagramaAnalisadoConsumer>(_loggerFactory.CreateLogger<ProcessamentoDiagramaAnalisadoConsumer>());
        var gateway = _serviceProvider.GetRequiredService<IResultadoDiagramaGateway>();
        var metrics = _serviceProvider.GetRequiredService<IMetricsService>();

        logger.LogInformation($"Recebida mensagem de processamento analisado para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);

        var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

        if (resultadoDiagrama == null)
        {
            logger.LogWarning($"Resultado de diagrama não encontrado para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}, ignorando", mensagem.AnaliseDiagramaId);
            return;
        }

        var analiseResultado = AnaliseResultado.Criar(mensagem.DescricaoAnalise, mensagem.ComponentesIdentificados, mensagem.RiscosArquiteturais, mensagem.RecomendacoesBasicas);

        var relatorioTecnico = JsonSerializer.Serialize(new
        {
            mensagem.DescricaoAnalise,
            mensagem.ComponentesIdentificados,
            mensagem.RiscosArquiteturais,
            mensagem.RecomendacoesBasicas,
            mensagem.DataConclusao
        });

        resultadoDiagrama.RegistrarAnalise(analiseResultado, relatorioTecnico);

        await gateway.SalvarAsync(resultadoDiagrama);

        metrics.RegistrarAnaliseConcluida(mensagem.AnaliseDiagramaId);

        logger.LogInformation($"Análise registrada com sucesso para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", mensagem.AnaliseDiagramaId);
    }
}
