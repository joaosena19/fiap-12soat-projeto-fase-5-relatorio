using Application.Contracts.Armazenamento;
using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Infrastructure.Relatorios.Pdf;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using Shared.Constants;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato PDF com upload para S3.
/// </summary>
public class RelatorioPdfStrategy : BaseRelatorioStrategy
{
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;

    public RelatorioPdfStrategy(IArmazenamentoArquivoService armazenamentoArquivoService, ILoggerFactory loggerFactory) : base(loggerFactory.CriarAppLogger<RelatorioPdfStrategy>())
    {
        _armazenamentoArquivoService = armazenamentoArquivoService;
    }

    public override TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Pdf;

    protected override async Task<ConteudosRelatorio> GerarConteudoAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama, AnaliseResultado analise)
    {
        var nomeArquivo = $"{resultadoDiagrama.AnaliseDiagramaId}/relatorio.pdf";
        byte[] pdfBytes;

        try
        {
            var documento = new RelatorioAnalisePdfDocumento(analise);
            pdfBytes = documento.GeneratePdf();
        }
        catch (Exception ex)
        {
            CriarLoggerContextualizado(resultadoDiagrama, nomeArquivo).LogError(ex, $"Erro ao gerar conteúdo do relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }

        try
        {
            var url = await _armazenamentoArquivoService.ArmazenarAsync(resultadoDiagrama.AnaliseDiagramaId, pdfBytes, nomeArquivo, "application/pdf");

            return ConteudosRelatorio.Vazio().Adicionar(ConteudoRelatorioChaves.Url, url);
        }
        catch (Exception ex)
        {
            CriarLoggerContextualizado(resultadoDiagrama, nomeArquivo).LogError(ex, $"Erro ao salvar no S3 o relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }
    }
}