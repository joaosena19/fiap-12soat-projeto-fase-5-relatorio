using Domain.ResultadoDiagrama.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Relatorios.Pdf;

/// <summary>
/// Documento QuestPDF para geração do relatório técnico de análise de diagrama.
/// </summary>
public class RelatorioAnalisePdfDocumento : IDocument
{
    private const string TituloPrincipal = "Relatório Técnico de Análise de Diagrama";
    private const string TituloSecaoDescricao = "Descrição da Análise";
    private const string TituloSecaoComponentes = "Componentes Identificados";
    private const string TituloSecaoRiscos = "Riscos Arquiteturais";
    private const string TituloSecaoRecomendacoes = "Recomendações Básicas";
    private const string RotuloPagina = "Página ";
    private const string SeparadorPagina = " de ";
    private const int MargemPagina = 40;
    private const int TamanhoFontePadrao = 11;
    private const int TamanhoFonteTituloPrincipal = 20;
    private const int TamanhoFonteTituloSecao = 14;
    private const int PaddingTopoLinhaCabecalho = 5;
    private const int PaddingTopoConteudo = 15;
    private const int PaddingBottomSecao = 10;
    private const int PaddingLeftItemLista = 10;

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
            page.Margin(MargemPagina);
            page.DefaultTextStyle(estilo => estilo.FontSize(TamanhoFontePadrao));

            page.Header().Column(coluna =>
            {
                coluna.Item().Text(TituloPrincipal).FontSize(TamanhoFonteTituloPrincipal).Bold().FontColor(Colors.Blue.Darken2);
                coluna.Item().PaddingTop(PaddingTopoLinhaCabecalho).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });

            page.Content().PaddingTop(PaddingTopoConteudo).Column(coluna =>
            {
                ComporSecaoDescricao(coluna);
                ComporSecaoComponentes(coluna);
                ComporSecaoRiscos(coluna);
                ComporSecaoRecomendacoes(coluna);
            });

            page.Footer().AlignCenter().Text(texto =>
            {
                texto.Span(RotuloPagina);
                texto.CurrentPageNumber();
                texto.Span(SeparadorPagina);
                texto.TotalPages();
            });
        });
    }

    private void ComporSecaoDescricao(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(PaddingBottomSecao).Column(secao =>
        {
            secao.Item().Text(TituloSecaoDescricao).FontSize(TamanhoFonteTituloSecao).Bold().FontColor(Colors.Blue.Darken1);
            secao.Item().PaddingTop(PaddingTopoLinhaCabecalho).Text(_analiseResultado.DescricaoAnalise.Valor);
        });
    }

    private void ComporSecaoComponentes(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(PaddingBottomSecao).Column(secao =>
        {
            secao.Item().Text(TituloSecaoComponentes).FontSize(TamanhoFonteTituloSecao).Bold().FontColor(Colors.Blue.Darken1);
            secao.Item().PaddingTop(PaddingTopoLinhaCabecalho).Column(lista =>
            {
                foreach (var componente in _analiseResultado.ComponentesIdentificados)
                    lista.Item().PaddingLeft(PaddingLeftItemLista).Text($"• {componente.Valor}");
            });
        });
    }

    private void ComporSecaoRiscos(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(PaddingBottomSecao).Column(secao =>
        {
            secao.Item().Text(TituloSecaoRiscos).FontSize(TamanhoFonteTituloSecao).Bold().FontColor(Colors.Red.Darken1);
            secao.Item().PaddingTop(PaddingTopoLinhaCabecalho).Column(lista =>
            {
                foreach (var risco in _analiseResultado.RiscosArquiteturais)
                    lista.Item().PaddingLeft(PaddingLeftItemLista).Text($"• {risco.Valor}");
            });
        });
    }

    private void ComporSecaoRecomendacoes(ColumnDescriptor coluna)
    {
        coluna.Item().PaddingBottom(PaddingBottomSecao).Column(secao =>
        {
            secao.Item().Text(TituloSecaoRecomendacoes).FontSize(TamanhoFonteTituloSecao).Bold().FontColor(Colors.Green.Darken2);
            secao.Item().PaddingTop(PaddingTopoLinhaCabecalho).Column(lista =>
            {
                foreach (var recomendacao in _analiseResultado.RecomendacoesBasicas)
                    lista.Item().PaddingLeft(PaddingLeftItemLista).Text($"• {recomendacao.Valor}");
            });
        });
    }
}
