using Application.Contracts.Armazenamento;
using Application.Contracts.Monitoramento;
using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Aggregates;
using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using System.Diagnostics;
using System.Text;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato Markdown com upload para S3.
/// </summary>
public class RelatorioMarkdownStrategy : IRelatorioStrategy
{
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;
    private readonly IAppLogger _logger;

    public RelatorioMarkdownStrategy(IArmazenamentoArquivoService armazenamentoArquivoService, ILoggerFactory loggerFactory)
    {
        _armazenamentoArquivoService = armazenamentoArquivoService;
        _logger = new LoggerAdapter<RelatorioMarkdownStrategy>(loggerFactory.CreateLogger<RelatorioMarkdownStrategy>());
    }

    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Markdown;

    public async Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var cronometro = Stopwatch.StartNew();
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException("Análise não está disponível para gerar markdown");
        var nomeArquivo = $"{resultadoDiagrama.AnaliseDiagramaId}/relatorio.md";

        _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogDebug($"Iniciando geração de relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);

        string markdownString;
        byte[] markdownBytes;

        try
        {
            markdownString = ConstruirMarkdown(analise);
            markdownBytes = Encoding.UTF8.GetBytes(markdownString);
        }
        catch (Exception ex)
        {
            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogError(ex, $"Erro ao gerar conteúdo do relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }

        try
        {
            var url = await _armazenamentoArquivoService.ArmazenarAsync(resultadoDiagrama.AnaliseDiagramaId, markdownBytes, nomeArquivo, "text/markdown");

            var conteudos = ConteudosRelatorio.Vazio()
                .Adicionar(ConteudoRelatorioChaves.InlineMarkdown, markdownString)
                .Adicionar(ConteudoRelatorioChaves.Url, url);

            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogDebug($"Relatório {{{LogNomesPropriedades.TipoRelatorio}}} gerado para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId, cronometro.ElapsedMilliseconds);

            return conteudos;
        }
        catch (Exception ex)
        {
            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogError(ex, $"Erro ao salvar no S3 o relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }
    }

    private static string ConstruirMarkdown(Domain.AnaliseDiagrama.Entities.AnaliseResultado analise)
    {
        var builder = new StringBuilder();

        builder.AppendLine("# Relatório Técnico");
        builder.AppendLine();
        builder.AppendLine($"**Análise:** {analise.DescricaoAnalise.Valor}");
        builder.AppendLine();
        builder.AppendLine("## Componentes Identificados");

        foreach (var item in analise.ComponentesIdentificados)
            builder.AppendLine($"- {item.Valor}");

        builder.AppendLine();
        builder.AppendLine("## Riscos Arquiteturais");

        foreach (var item in analise.RiscosArquiteturais)
            builder.AppendLine($"- {item.Valor}");

        builder.AppendLine();
        builder.AppendLine("## Recomendações Básicas");

        foreach (var item in analise.RecomendacoesBasicas)
            builder.AppendLine($"- {item.Valor}");

        return builder.ToString();
    }
}