using Domain.ResultadoDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class ErroResultadoDiagramaDto
{
    public string Mensagem { get; set; } = string.Empty;
    public TipoRelatorioEnum? TipoRelatorio { get; set; }
    public DateTimeOffset DataOcorrencia { get; set; }
}
