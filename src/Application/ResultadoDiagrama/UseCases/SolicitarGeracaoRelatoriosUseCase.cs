using Application.Contracts.Gateways;
using Application.Contracts.Messaging;
using Application.Contracts.Monitoramento;
using Application.Contracts.Presenters;
using Application.Extensions;
using Application.ResultadoDiagrama.Dtos;
using Domain.ResultadoDiagrama.Enums;
using Shared.Constants;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.ResultadoDiagrama.UseCases;

/// <summary>
/// Solicita a geração assíncrona de relatórios complementares.
/// </summary>
public class SolicitarGeracaoRelatoriosUseCase
{
    public async Task ExecutarAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio, IResultadoDiagramaGateway gateway, IRelatorioMessagePublisher messagePublisher, ISolicitarGeracaoRelatoriosPresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ValidarTiposRelatorio(tiposRelatorio, presenter))
                return;

            logger.ComUseCase(this).LogDebug($"Solicitando geração de relatórios para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);

            var resultadoDiagrama = await ObterResultadoDiagramaAsync(analiseDiagramaId, gateway, presenter);
            if (resultadoDiagrama == null)
                return;

            var resultadoSolicitacao = MontarResultadoSolicitacao(analiseDiagramaId, tiposRelatorio, resultadoDiagrama);
            await PersistirEPublicarSolicitacoesAsync(analiseDiagramaId, resultadoDiagrama, resultadoSolicitacao.Relatorios, gateway, messagePublisher);
            presenter.ApresentarSucesso(resultadoSolicitacao);
        }
        catch (DomainException ex)
        {
            logger.ComUseCase(this).ComDomainErrorType(ex).LogInformation(ex.LogTemplate, ex.LogArgs);
            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this).LogError(ex, $"Erro inesperado ao solicitar relatórios para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);
            presenter.ApresentarErro("Erro interno ao solicitar geração de relatórios", ErrorType.UnexpectedError);
        }
    }

    private static bool ValidarTiposRelatorio(IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio, ISolicitarGeracaoRelatoriosPresenter presenter)
    {
        if (tiposRelatorio.Count == 0)
        {
            presenter.ApresentarErro("Ao menos um tipo de relatório deve ser informado", ErrorType.InvalidInput);
            return false;
        }

        var tiposInvalidos = tiposRelatorio.Where(item => !Enum.IsDefined(typeof(TipoRelatorioEnum), item)).ToList();
        if (tiposInvalidos.Count == 0)
            return true;

        presenter.ApresentarErro($"Tipo(s) de relatório inválido(s): {string.Join(", ", tiposInvalidos)}", ErrorType.InvalidInput);
        return false;
    }

    private static async Task<Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama?> ObterResultadoDiagramaAsync(Guid analiseDiagramaId, IResultadoDiagramaGateway gateway, ISolicitarGeracaoRelatoriosPresenter presenter)
    {
        var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);
        if (resultadoDiagrama != null)
            return resultadoDiagrama;

        presenter.ApresentarErro("Resultado de diagrama não encontrado", ErrorType.ResourceNotFound);
        return null;
    }

    private static ResultadoSolicitacaoRelatoriosDto MontarResultadoSolicitacao(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio, Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama)
    {
        var itensResposta = new List<ItemResultadoSolicitacaoRelatorioDto>();

        foreach (var tipoRelatorio in tiposRelatorio.Distinct())
            itensResposta.Add(CriarItemResultadoSolicitacao(tipoRelatorio, resultadoDiagrama));

        return new ResultadoSolicitacaoRelatoriosDto { AnaliseDiagramaId = analiseDiagramaId, Relatorios = itensResposta };
    }

    private static ItemResultadoSolicitacaoRelatorioDto CriarItemResultadoSolicitacao(TipoRelatorioEnum tipoRelatorio, Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama)
    {
        var resultado = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(tipoRelatorio);

        if (resultado == ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao)
            resultadoDiagrama.MarcarRelatorioSolicitado(tipoRelatorio);

        return new ItemResultadoSolicitacaoRelatorioDto { Tipo = tipoRelatorio, Resultado = resultado };
    }

    private static async Task PersistirEPublicarSolicitacoesAsync(Guid analiseDiagramaId, Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama, IReadOnlyCollection<ItemResultadoSolicitacaoRelatorioDto> relatorios, IResultadoDiagramaGateway gateway, IRelatorioMessagePublisher messagePublisher)
    {
        var tiposParaFila = relatorios
            .Where(item => item.Resultado == ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao)
            .Select(item => item.Tipo)
            .ToList();

        if (tiposParaFila.Count == 0)
            return;

        await gateway.SalvarAsync(resultadoDiagrama);
        await messagePublisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposParaFila);
    }
}