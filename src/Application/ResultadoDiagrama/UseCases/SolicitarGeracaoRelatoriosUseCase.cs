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
            if (tiposRelatorio.Count == 0)
            {
                presenter.ApresentarErro("Ao menos um tipo de relatório deve ser informado", ErrorType.InvalidInput);
                return;
            }

            var tiposInvalidos = tiposRelatorio.Where(item => !Enum.IsDefined(typeof(TipoRelatorioEnum), item)).ToList();
            if (tiposInvalidos.Count > 0)
            {
                presenter.ApresentarErro($"Tipo(s) de relatório inválido(s): {string.Join(", ", tiposInvalidos)}", ErrorType.InvalidInput);
                return;
            }

            logger.ComUseCase(this).LogDebug($"Solicitando geração de relatórios para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);

            var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);
            if (resultadoDiagrama == null)
            {
                presenter.ApresentarErro("Resultado de diagrama não encontrado", ErrorType.ResourceNotFound);
                return;
            }

            var itensResposta = new List<ItemResultadoSolicitacaoRelatorioDto>();
            var tiposParaFila = new List<TipoRelatorioEnum>();

            foreach (var tipoRelatorio in tiposRelatorio.Distinct())
            {
                var resultado = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(tipoRelatorio);

                if (resultado == ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao)
                {
                    resultadoDiagrama.MarcarRelatorioSolicitado(tipoRelatorio);
                    tiposParaFila.Add(tipoRelatorio);
                }

                itensResposta.Add(new ItemResultadoSolicitacaoRelatorioDto { Tipo = tipoRelatorio, Resultado = resultado });
            }

            if (tiposParaFila.Count > 0)
            {
                await gateway.SalvarAsync(resultadoDiagrama);
                await messagePublisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposParaFila);
            }

            presenter.ApresentarSucesso(new ResultadoSolicitacaoRelatoriosDto { AnaliseDiagramaId = analiseDiagramaId, Relatorios = itensResposta });
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
}