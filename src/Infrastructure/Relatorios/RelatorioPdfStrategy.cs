using Application.Contracts.Armazenamento;
using Application.Contracts.Monitoramento;
using Application.Contracts.Relatorios;
using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.Enums;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Infrastructure.Relatorios.Pdf;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using Shared.Constants;
using System.Diagnostics;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato PDF com upload para S3.
/// </summary>
public class RelatorioPdfStrategy : IRelatorioStrategy
{
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;
    private readonly IAppLogger _logger;

    public RelatorioPdfStrategy(IArmazenamentoArquivoService armazenamentoArquivoService, ILoggerFactory loggerFactory)
    {
        _armazenamentoArquivoService = armazenamentoArquivoService;
        _logger = new LoggerAdapter<RelatorioPdfStrategy>(loggerFactory.CreateLogger<RelatorioPdfStrategy>());
    }

    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Pdf;

    public async Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var cronometro = Stopwatch.StartNew();
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException("Análise não está disponível para gerar PDF");
        var nomeArquivo = $"{resultadoDiagrama.AnaliseDiagramaId}/relatorio.pdf";

        _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogDebug($"Iniciando geração de relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);

        byte[] pdfBytes;

        try
        {
            var documento = new RelatorioAnalisePdfDocumento(analise);
            pdfBytes = documento.GeneratePdf();
        }
        catch (Exception ex)
        {
            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogError(ex, $"Erro ao gerar conteúdo do relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }

        try
        {
            var url = await _armazenamentoArquivoService.ArmazenarAsync(resultadoDiagrama.AnaliseDiagramaId, pdfBytes, nomeArquivo, "application/pdf");

            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogDebug($"Relatório {{{LogNomesPropriedades.TipoRelatorio}}} gerado para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId, cronometro.ElapsedMilliseconds);

            return ConteudosRelatorio.Vazio().Adicionar(ConteudoRelatorioChaves.Url, url);
        }
        catch (Exception ex)
        {
            _logger.ComPropriedade(LogNomesPropriedades.TipoRelatorio, TipoRelatorio).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, resultadoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.NomeArquivo, nomeArquivo).LogError(ex, $"Erro ao salvar no S3 o relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }
    }
}