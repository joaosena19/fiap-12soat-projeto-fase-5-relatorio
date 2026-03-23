using Domain.ResultadoDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class RetornoResultadoDiagramaDto
{
    public Guid AnaliseDiagramaId { get; set; }
    public StatusAnaliseEnum Status { get; set; }
    public List<RelatorioDto> Relatorios { get; set; } = new();
    public List<ErroResultadoDiagramaDto> Erros { get; set; } = new();
    public DateTimeOffset DataCriacao { get; set; }
}