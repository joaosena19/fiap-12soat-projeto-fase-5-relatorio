namespace Application.ResultadoDiagrama.Dtos;

public class ResultadoSolicitacaoRelatoriosDto
{
    public Guid AnaliseDiagramaId { get; set; }
    public List<ItemResultadoSolicitacaoRelatorioDto> Relatorios { get; set; } = new();
}