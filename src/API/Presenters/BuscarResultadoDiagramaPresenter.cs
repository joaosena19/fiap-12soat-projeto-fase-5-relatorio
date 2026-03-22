using Application.Contracts.Presenters;
using Application.ResultadoDiagrama.Dtos;
using Domain.ResultadoDiagrama.Enums;

namespace API.Presenters;

public class BuscarResultadoDiagramaPresenter : BasePresenter, IBuscarResultadoDiagramaPresenter
{
    public void ApresentarSucesso(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama)
    {
        var dto = new RetornoResultadoDiagramaDto
        {
            AnaliseDiagramaId = resultadoDiagrama.AnaliseDiagramaId,
            Status = resultadoDiagrama.Status.Valor,
            Relatorios = resultadoDiagrama.Relatorios.Select(relatorio => new RelatorioDto
            {
                Tipo = relatorio.Tipo.Valor,
                Status = relatorio.Status.Valor,
                Conteudos = relatorio.Conteudos.Valores.ToDictionary(item => item.Key, item => item.Value),
                DataGeracao = relatorio.DataGeracao?.Valor
            }).ToList(),
            Erros = resultadoDiagrama.Erros.Select(erro => new ErroResultadoDiagramaDto
            {
                Mensagem = erro.Mensagem.Valor,
                TipoRelatorio = erro.TipoRelatorio.Valor,
                DataOcorrencia = erro.DataOcorrencia.Valor
            }).ToList(),
            DataCriacao = resultadoDiagrama.DataCriacao.Valor
        };

        DefinirSucesso(dto);
    }
}