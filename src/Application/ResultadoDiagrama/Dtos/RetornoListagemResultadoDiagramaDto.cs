using Domain.ResultadoDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class RetornoListagemResultadoDiagramaDto
{
    public Guid AnaliseDiagramaId { get; set; }
    public StatusAnaliseEnum Status { get; set; }
    public List<string> RelatoriosDisponiveis { get; set; } = new();
    public int QuantidadeErros { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
    public DateTimeOffset DataUltimaTentativa { get; set; }
}
