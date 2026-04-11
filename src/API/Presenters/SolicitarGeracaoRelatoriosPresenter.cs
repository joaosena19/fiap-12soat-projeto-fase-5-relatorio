using Application.Contracts.Presenters;
using Application.ResultadoDiagrama.Dtos;
using Domain.ResultadoDiagrama.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Presenters;

public class SolicitarGeracaoRelatoriosPresenter : BasePresenter, ISolicitarGeracaoRelatoriosPresenter
{
    private static readonly Dictionary<ResultadoSolicitacaoGeracaoRelatorioEnum, (int StatusHttp, string Mensagem)> MapeamentoResultados = new()
    {
        [ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido] = (StatusCodes.Status200OK, "Relatório já gerado. Consulte o endpoint GET para obter o conteúdo"),
        [ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao] = (StatusCodes.Status202Accepted, "Solicitação aceita. O relatório será processado em breve"),
        [ResultadoSolicitacaoGeracaoRelatorioEnum.JaEmAndamento] = (StatusCodes.Status202Accepted, "O relatório já está sendo processado. Consulte o endpoint GET em alguns instantes")
    };

    public void ApresentarSucesso(ResultadoSolicitacaoRelatoriosDto resultado)
    {
        var itensResposta = resultado.Relatorios.Select(item =>
        {
            var (statusHttp, mensagem) = MapeamentoResultados[item.Resultado];
            return new { item.Tipo, StatusHttp = statusHttp, Mensagem = mensagem, item.Resultado };
        }).ToList();

        var resposta = new { resultado.AnaliseDiagramaId, Relatorios = itensResposta };
        var statusCodeHttp = DeterminarStatusHttp(itensResposta.Select(item => item.StatusHttp).ToList());

        _resultado = statusCodeHttp switch
        {
            StatusCodes.Status202Accepted => new ObjectResult(resposta) { StatusCode = StatusCodes.Status202Accepted },
            StatusCodes.Status207MultiStatus => new ObjectResult(resposta) { StatusCode = StatusCodes.Status207MultiStatus },
            _ => new OkObjectResult(resposta)
        };
        _foiSucesso = true;
    }

    private static int DeterminarStatusHttp(IReadOnlyCollection<int> statusCodes)
    {
        if (statusCodes.All(item => item == StatusCodes.Status200OK))
            return StatusCodes.Status200OK;

        if (statusCodes.All(item => item == StatusCodes.Status202Accepted))
            return StatusCodes.Status202Accepted;

        return StatusCodes.Status207MultiStatus;
    }
}