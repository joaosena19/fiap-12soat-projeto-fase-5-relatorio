using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IListarResultadosDiagramaPresenter
{
    void ApresentarSucesso(IReadOnlyCollection<Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama> resultados);
    void ApresentarErro(string mensagem, ErrorType errorType);
}
