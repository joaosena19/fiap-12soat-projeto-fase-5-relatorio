using Application.ResultadoDiagrama.Dtos;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface ISolicitarGeracaoRelatoriosPresenter
{
    void ApresentarErro(string mensagem, ErrorType errorType);
    void ApresentarSucesso(ResultadoSolicitacaoRelatoriosDto resultado);
}