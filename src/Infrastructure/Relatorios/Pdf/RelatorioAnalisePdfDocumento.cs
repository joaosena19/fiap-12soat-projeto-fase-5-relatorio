using Domain.AnaliseDiagrama.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Relatorios.Pdf;

/// <summary>
/// Documento QuestPDF para geração do relatório técnico de análise de diagrama.
/// </summary>
public class RelatorioAnalisePdfDocumento : IDocument
{
    private readonly AnaliseResultado _analiseResultado;

    public RelatorioAnalisePdfDocumento(AnaliseResultado analiseResultado)
    {
        _analiseResultado = analiseResultado;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.DefaultTextStyle(estilo => estilo.FontSize(11));

            page.Header().Column(coluna =>
            {
                coluna.Item().Text("Relatório Técnico de Análise de Diagrama").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                coluna.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });

            page.Content().PaddingTop(15).Column(coluna =>
            {
                ComporSecaoDescricao(coluna);
                ComporSecaoComponentes(coluna);
                ComporSecaoRiscos(coluna);
                ComporSecaoRecomendacoes(coluna);
            });

            page.Footer().AlignCenter().Text(texto =>
            {
                texto.Span("Página ");
                texto.CurrentPageNumber();
                texto.Span(" de ");
                texto.TotalPages();
            });
        });
    }

    private void ComporSecaoDescricao(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(10).Column(secao =>
        {
            secao.Item().Text("Descrição da Análise").FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
            secao.Item().PaddingTop(5).Text(_analiseResultado.DescricaoAnalise.Valor);
        });
    }

    private void ComporSecaoComponentes(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(10).Column(secao =>
        {
            secao.Item().Text("Componentes Identificados").FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
            secao.Item().PaddingTop(5).Column(lista =>
            {
                foreach (var componente in _analiseResultado.ComponentesIdentificados)
                    lista.Item().PaddingLeft(10).Text($"• {componente.Valor}");
            });
        });
    }

    private void ComporSecaoRiscos(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(10).Column(secao =>
        {
            secao.Item().Text("Riscos Arquiteturais").FontSize(14).Bold().FontColor(Colors.Red.Darken1);
            secao.Item().PaddingTop(5).Column(lista =>
            {
                foreach (var risco in _analiseResultado.RiscosArquiteturais)
                    lista.Item().PaddingLeft(10).Text($"• {risco.Valor}");
            });
        });
    }

    private void ComporSecaoRecomendacoes(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(10).Column(secao =>
        {
            secao.Item().Text("Recomendações Básicas").FontSize(14).Bold().FontColor(Colors.Green.Darken2);
            secao.Item().PaddingTop(5).Column(lista =>
            {
                foreach (var recomendacao in _analiseResultado.RecomendacoesBasicas)
                    lista.Item().PaddingLeft(10).Text($"• {recomendacao.Valor}");
            });
        });
    }
}
