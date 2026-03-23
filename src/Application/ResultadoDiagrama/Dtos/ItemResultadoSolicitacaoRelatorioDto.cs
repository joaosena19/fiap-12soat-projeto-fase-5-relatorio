using Domain.ResultadoDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class ItemResultadoSolicitacaoRelatorioDto
{
    public TipoRelatorioEnum Tipo { get; set; }
    public ResultadoSolicitacaoGeracaoRelatorioEnum Resultado { get; set; }
}
