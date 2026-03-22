using Application.Contracts.Armazenamento;
using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Aggregates;
using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Shared.Constants;
using System.Text;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato Markdown com upload para S3.
/// </summary>
public class RelatorioMarkdownStrategy : IRelatorioStrategy
{
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;

    public RelatorioMarkdownStrategy(IArmazenamentoArquivoService armazenamentoArquivoService)
    {
        _armazenamentoArquivoService = armazenamentoArquivoService;
    }

    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Markdown;

    public async Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException("Análise não está disponível para gerar markdown");

        var markdownString = ConstruirMarkdown(analise);
        var markdownBytes = Encoding.UTF8.GetBytes(markdownString);
        var nomeArquivo = $"{resultadoDiagrama.AnaliseDiagramaId}/relatorio.md";
        var url = await _armazenamentoArquivoService.ArmazenarAsync(markdownBytes, nomeArquivo, "text/markdown");

        var conteudos = ConteudosRelatorio.Vazio()
            .Adicionar(ConteudoRelatorioChaves.InlineMarkdown, markdownString)
            .Adicionar(ConteudoRelatorioChaves.Url, url);

        return conteudos;
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