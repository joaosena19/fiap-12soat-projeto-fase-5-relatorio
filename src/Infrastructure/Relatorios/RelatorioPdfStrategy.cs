using Application.Contracts.Armazenamento;
using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Aggregates;
using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Infrastructure.Relatorios.Pdf;
using QuestPDF.Fluent;
using Shared.Constants;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato PDF com upload para S3.
/// </summary>
public class RelatorioPdfStrategy : IRelatorioStrategy
{
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;

    public RelatorioPdfStrategy(IArmazenamentoArquivoService armazenamentoArquivoService)
    {
        _armazenamentoArquivoService = armazenamentoArquivoService;
    }

    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Pdf;

    public async Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException("Análise não está disponível para gerar PDF");

        var documento = new RelatorioAnalisePdfDocumento(analise);
        var pdfBytes = documento.GeneratePdf();
        var nomeArquivo = $"{resultadoDiagrama.AnaliseDiagramaId}/relatorio.pdf";
        var url = await _armazenamentoArquivoService.ArmazenarAsync(pdfBytes, nomeArquivo, "application/pdf");

        return ConteudosRelatorio.Vazio().Adicionar(ConteudoRelatorioChaves.Url, url);
    }
}