using Application.Contracts.Monitoramento;
using Application.Contracts.Relatorios;
using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Shared.Constants;
using System.Diagnostics;

namespace Infrastructure.Relatorios;

/// <summary>
/// Classe base para estratégias de geração de relatório, encapsulando logging e cronometragem comuns.
/// </summary>
public abstract class BaseRelatorioStrategy : IRelatorioStrategy
{
    protected readonly IAppLogger _logger;

    protected BaseRelatorioStrategy(IAppLogger logger)
    {
        _logger = logger;
    }

    public abstract TipoRelatorioEnum TipoRelatorio { get; }

    public async Task<ConteudosRelatorio> GerarAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama)
    {
        var cronometro = Stopwatch.StartNew();
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException($"Análise não está disponível para gerar {TipoRelatorio}");

        CriarLoggerContextualizado(resultadoDiagrama).LogDebug($"Iniciando geração de relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);

        var resultado = await GerarConteudoAsync(resultadoDiagrama, analise);

        CriarLoggerContextualizado(resultadoDiagrama).LogDebug($"Relatório {{{LogNomesPropriedades.TipoRelatorio}}} gerado para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId, cronometro.ElapsedMilliseconds);

        return resultado;
    }

    protected abstract Task<ConteudosRelatorio> GerarConteudoAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama, AnaliseResultado analise);

    protected IAppLogger CriarLoggerContextualizado(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama)
    {
        return _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio)
                      .ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId);
    }

    protected IAppLogger CriarLoggerContextualizado(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama, string nomeArquivo)
    {
        return CriarLoggerContextualizado(resultadoDiagrama)
                      .ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo);
    }
}
