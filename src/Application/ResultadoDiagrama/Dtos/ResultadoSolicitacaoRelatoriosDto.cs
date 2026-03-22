using Domain.AnaliseDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class ResultadoSolicitacaoRelatoriosDto
{
    public Guid AnaliseDiagramaId { get; set; }
    public List<ItemResultadoSolicitacaoRelatorioDto> Relatorios { get; set; } = new();
}

public class ItemResultadoSolicitacaoRelatorioDto
{
    public TipoRelatorioEnum Tipo { get; set; }
    public ResultadoSolicitacaoGeracaoRelatorioEnum Resultado { get; set; }
}