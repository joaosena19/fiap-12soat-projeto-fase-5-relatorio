using Application.Contracts.Gateways;
using Application.Contracts.Monitoramento;
using Application.Contracts.Relatorios;
using Application.Extensions;
using Domain.AnaliseDiagrama.Enums;
using Shared.Constants;

namespace Application.ResultadoDiagrama.UseCases;

/// <summary>
/// Executa a geração efetiva de um relatório sob demanda via strategy.
/// </summary>
public class GerarRelatorioUseCase
{
    public async Task ExecutarAsync(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio, IResultadoDiagramaGateway gateway, IRelatorioStrategyResolver strategyResolver, IMetricsService metrics, IAppLogger logger)
    {
        logger.ComUseCase(this).LogInformation($"Iniciando geração de relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", tipoRelatorio, analiseDiagramaId);

        var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);
        if (resultadoDiagrama == null)
        {
            logger.ComUseCase(this).LogWarning($"Resultado de diagrama não encontrado para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);
            return;
        }

        var relatorio = resultadoDiagrama.ObterRelatorio(tipoRelatorio);
        if (!relatorio.PodeGerar())
        {
            logger.ComUseCase(this).LogDebug($"Relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}} não pode ser gerado (estado atual não permite)", tipoRelatorio, analiseDiagramaId);
            return;
        }

        if (!resultadoDiagrama.AnaliseDisponivel())
        {
            logger.ComUseCase(this).LogWarning($"Solicitação ignorada para relatório {{{LogNomesPropriedades.TipoRelatorio}}} em {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}} porque a análise ainda não está disponível", tipoRelatorio, analiseDiagramaId);
            return;
        }

        try
        {
            resultadoDiagrama.MarcarRelatorioEmProcessamento(tipoRelatorio);
            await gateway.SalvarAsync(resultadoDiagrama);

            var strategy = strategyResolver.Resolver(tipoRelatorio);
            var conteudo = await strategy.GerarAsync(resultadoDiagrama);

            resultadoDiagrama.ConcluirRelatorio(tipoRelatorio, conteudo);
            await gateway.SalvarAsync(resultadoDiagrama);

            metrics.RegistrarRelatorioGerado(analiseDiagramaId, tipoRelatorio);
            logger.ComUseCase(this).LogInformation($"Relatório {{{LogNomesPropriedades.TipoRelatorio}}} gerado com sucesso para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", tipoRelatorio, analiseDiagramaId);
        }
        catch (Exception ex)
        {
            resultadoDiagrama.RegistrarFalhaRelatorio(tipoRelatorio, ex.Message);
            await gateway.SalvarAsync(resultadoDiagrama);

            metrics.RegistrarRelatorioComFalha(analiseDiagramaId, tipoRelatorio, ex.Message);
            logger.ComUseCase(this).LogError(ex, $"Erro ao gerar relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", tipoRelatorio, analiseDiagramaId);
        }
    }
}
