using Application.Contracts.Monitoramento;
using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Aggregates;
using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using System.Diagnostics;
using System.Text.Json;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato JSON a partir da análise do diagrama.
/// </summary>
public class RelatorioJsonStrategy : IRelatorioStrategy
{
    private readonly IAppLogger _logger;

    public RelatorioJsonStrategy(ILoggerFactory loggerFactory)
    {
        _logger = new LoggerAdapter<RelatorioJsonStrategy>(loggerFactory.CreateLogger<RelatorioJsonStrategy>());
    }

    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Json;

    public Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var cronometro = Stopwatch.StartNew();
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException("Análise não está disponível para gerar JSON");

        _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).LogDebug($"Iniciando geração de relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);

        try
        {
            var jsonString = JsonSerializer.Serialize(new
            {
                DescricaoAnalise = analise.DescricaoAnalise.Valor,
                ComponentesIdentificados = analise.ComponentesIdentificados.Select(item => item.Valor).ToList(),
                RiscosArquiteturais = analise.RiscosArquiteturais.Select(item => item.Valor).ToList(),
                RecomendacoesBasicas = analise.RecomendacoesBasicas.Select(item => item.Valor).ToList()
            });

            var conteudos = ConteudosRelatorio.Vazio().Adicionar(ConteudoRelatorioChaves.JsonString, jsonString);

            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).LogDebug($"Relatório {{{LogNomesPropriedades.TipoRelatorio}}} gerado para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId, cronometro.ElapsedMilliseconds);

            return Task.FromResult(conteudos);
        }
        catch (Exception ex)
        {
            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).LogError(ex, $"Erro ao gerar conteúdo do relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }
    }
}
