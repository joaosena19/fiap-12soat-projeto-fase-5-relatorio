using Application.Contracts.Armazenamento;
using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using System.Text;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato Markdown com upload para S3.
/// </summary>
public class RelatorioMarkdownStrategy : BaseRelatorioStrategy
{
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;

    public RelatorioMarkdownStrategy(IArmazenamentoArquivoService armazenamentoArquivoService, ILoggerFactory loggerFactory) : base(loggerFactory.CriarAppLogger<RelatorioMarkdownStrategy>())
    {
        _armazenamentoArquivoService = armazenamentoArquivoService;
    }

    public override TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Markdown;

    protected override async Task<ConteudosRelatorio> GerarConteudoAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama, AnaliseResultado analise)
    {
        var nomeArquivo = $"{resultadoDiagrama.AnaliseDiagramaId}/relatorio.md";

        string markdownString;
        byte[] markdownBytes;

        try
        {
            markdownString = ConstruirMarkdown(analise);
            markdownBytes = Encoding.UTF8.GetBytes(markdownString);
        }
        catch (Exception ex)
        {
            CriarLoggerContextualizado(resultadoDiagrama, nomeArquivo).LogError(ex, $"Erro ao gerar conteúdo do relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }

        try
        {
            var url = await _armazenamentoArquivoService.ArmazenarAsync(resultadoDiagrama.AnaliseDiagramaId, markdownBytes, nomeArquivo, "text/markdown; charset=utf-8");

            return ConteudosRelatorio.Vazio()
                .Adicionar(ConteudoRelatorioChaves.InlineMarkdown, markdownString)
                .Adicionar(ConteudoRelatorioChaves.Url, url);
        }
        catch (Exception ex)
        {
            CriarLoggerContextualizado(resultadoDiagrama, nomeArquivo).LogError(ex, $"Erro ao salvar no S3 o relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }
    }

    private static string ConstruirMarkdown(AnaliseResultado analise)
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