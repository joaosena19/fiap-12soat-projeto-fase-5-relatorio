using Application.Contracts.Presenters;
using Application.ResultadoDiagrama.Dtos;
using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.Enums;

namespace API.Presenters;

public class ListarResultadosDiagramaPresenter : BasePresenter, IListarResultadosDiagramaPresenter
{
    public void ApresentarSucesso(IReadOnlyCollection<ResultadoDiagrama> resultados)
    {
        var dto = resultados.Select(CriarDto).ToList();
        DefinirSucesso(dto);
    }

    private static RetornoListagemResultadoDiagramaDto CriarDto(ResultadoDiagrama resultado)
    {
        return new RetornoListagemResultadoDiagramaDto
        {
            AnaliseDiagramaId = resultado.AnaliseDiagramaId,
            Status = resultado.Status.Valor,
            RelatoriosDisponiveis = resultado.Relatorios
                .Where(r => r.Status.Valor == StatusRelatorioEnum.Concluido)
                .Select(r => r.Tipo.Valor.ToString())
                .ToList(),
            QuantidadeErros = resultado.Erros.Count,
            DataCriacao = resultado.DataCriacao.Valor
        };
    }
}
