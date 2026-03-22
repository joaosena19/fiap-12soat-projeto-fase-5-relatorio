using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Aggregates;
using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Shared.Constants;
using System.Text;

namespace Infrastructure.Relatorios;

public class RelatorioMarkdownStrategy : IRelatorioStrategy
{
    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Markdown;

    public Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException("Análise não está disponível para gerar markdown");

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

        var markdownString = builder.ToString();
        var conteudos = ConteudosRelatorio.Vazio()
            .Adicionar(ConteudoRelatorioChaves.Url, $"data:text/markdown;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes(markdownString))}")
            .Adicionar(ConteudoRelatorioChaves.InlineMarkdown, markdownString);

        return Task.FromResult(conteudos);
    }
}