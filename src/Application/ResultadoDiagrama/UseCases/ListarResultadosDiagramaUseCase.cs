using Application.Contracts.Gateways;
using Application.Contracts.Monitoramento;
using Application.Contracts.Presenters;
using Application.Extensions;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.ResultadoDiagrama.UseCases;

/// <summary>
/// Lista todos os resultados de diagramas.
/// </summary>
public class ListarResultadosDiagramaUseCase
{
    public async Task ExecutarAsync(IResultadoDiagramaGateway gateway, IListarResultadosDiagramaPresenter presenter, IAppLogger logger)
    {
        try
        {
            logger.ComUseCase(this).LogDebug("Listando resultados de diagramas");

            var resultados = await gateway.ListarAsync();
            presenter.ApresentarSucesso(resultados);
        }
        catch (DomainException ex)
        {
            logger.ComUseCase(this).ComDomainErrorType(ex).LogInformation(ex.LogTemplate, ex.LogArgs);
            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this).LogError(ex, "Erro inesperado ao listar resultados de diagramas");
            presenter.ApresentarErro("Erro interno ao listar resultados de diagramas", ErrorType.UnexpectedError);
        }
    }
}
