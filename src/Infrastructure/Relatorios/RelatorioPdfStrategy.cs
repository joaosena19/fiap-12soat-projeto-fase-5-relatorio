using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Aggregates;
using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Shared.Constants;
using System.Text;

namespace Infrastructure.Relatorios;

public class RelatorioPdfStrategy : IRelatorioStrategy
{
    private readonly IRelatorioStrategyResolver _strategyResolver;

    public RelatorioPdfStrategy(IRelatorioStrategyResolver strategyResolver)
    {
        _strategyResolver = strategyResolver;
    }

    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Pdf;

    public async Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var markdownStrategy = _strategyResolver.Resolver(TipoRelatorioEnum.Markdown);
        var markdownConteudos = await markdownStrategy.GerarAsync(resultadoDiagrama);

        var inlineMarkdown = markdownConteudos.ObterValor(ConteudoRelatorioChaves.InlineMarkdown) ?? throw new InvalidOperationException("Conteúdo markdown não está disponível para gerar PDF");
        var pdfBytes = GerarPdfSimples(inlineMarkdown);
        var url = $"data:application/pdf;base64,{Convert.ToBase64String(pdfBytes)}";

        return ConteudosRelatorio.Vazio().Adicionar(ConteudoRelatorioChaves.Url, url);
    }

    private static byte[] GerarPdfSimples(string texto)
    {
        var linhas = texto.Split(Environment.NewLine, StringSplitOptions.None)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Replace("(", "\\(").Replace(")", "\\)").Replace("\\", "\\\\"))
            .ToList();

        var conteudo = new StringBuilder();
        conteudo.AppendLine("BT");
        conteudo.AppendLine("/F1 12 Tf");
        conteudo.AppendLine("50 780 Td");

        foreach (var linha in linhas)
            conteudo.AppendLine($"({linha}) Tj T*");

        conteudo.AppendLine("ET");

        var stream = conteudo.ToString();
        var objetos = new List<string>
        {
            "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj",
            "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj",
            "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >> endobj",
            "4 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj",
            $"5 0 obj << /Length {Encoding.ASCII.GetByteCount(stream)} >> stream\n{stream}endstream endobj"
        };

        var builder = new StringBuilder();
        builder.AppendLine("%PDF-1.4");

        var offsets = new List<int> { 0 };

        foreach (var objeto in objetos)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(builder.ToString()));
            builder.AppendLine(objeto);
        }

        var xrefOffset = Encoding.ASCII.GetByteCount(builder.ToString());
        builder.AppendLine("xref");
        builder.AppendLine($"0 {objetos.Count + 1}");
        builder.AppendLine("0000000000 65535 f ");

        foreach (var offset in offsets.Skip(1))
            builder.AppendLine($"{offset:D10} 00000 n ");

        builder.AppendLine("trailer << /Size 6 /Root 1 0 R >>");
        builder.AppendLine("startxref");
        builder.AppendLine(xrefOffset.ToString());
        builder.Append("%%EOF");

        return Encoding.ASCII.GetBytes(builder.ToString());
    }
}