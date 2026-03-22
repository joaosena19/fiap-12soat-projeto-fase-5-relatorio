using Application.Contracts.Gateways;
using Application.Contracts.Monitoramento;
using Application.Contracts.Presenters;
using Application.Extensions;
using Shared.Constants;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.ResultadoDiagrama.UseCases;

/// <summary>
/// Busca um resultado de diagrama pelo AnaliseDiagramaId.
/// </summary>
public class BuscarResultadoDiagramaPorIdUseCase
{
    public async Task ExecutarAsync(Guid analiseDiagramaId, IResultadoDiagramaGateway gateway, IBuscarResultadoDiagramaPresenter presenter, IAppLogger logger)
    {
        try
        {
            logger.ComUseCase(this).LogDebug($"Buscando resultado de diagrama para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);

            var resultadoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);

            if (resultadoDiagrama == null)
            {
                presenter.ApresentarErro("Resultado de diagrama não encontrado para o identificador informado", ErrorType.ResourceNotFound);
                return;
            }

            presenter.ApresentarSucesso(resultadoDiagrama);
        }
        catch (DomainException ex)
        {
            logger.ComUseCase(this).ComDomainErrorType(ex).LogInformation(ex.LogTemplate, ex.LogArgs);
            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this).LogError(ex, $"Erro inesperado ao buscar resultado de diagrama para {LogNomesPropriedades.AnaliseDiagramaId} {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);
            presenter.ApresentarErro("Erro interno ao buscar resultado de diagrama", ErrorType.UnexpectedError);
        }
    }
}