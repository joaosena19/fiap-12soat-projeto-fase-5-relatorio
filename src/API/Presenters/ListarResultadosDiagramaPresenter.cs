using Application.Contracts.Presenters;
using Application.ResultadoDiagrama.Dtos;
using Domain.ResultadoDiagrama.Aggregates;

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
            Relatorios = resultado.Relatorios.Select(r => new RelatorioResumoDto
            {
                Tipo = r.Tipo.Valor,
                Status = r.Status.Valor
            }).ToList(),
            QuantidadeErros = resultado.Erros.Count,
            DataCriacao = resultado.DataCriacao.Valor
        };
    }
}
