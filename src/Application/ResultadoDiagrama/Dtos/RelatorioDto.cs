using Domain.ResultadoDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class RelatorioDto
{
    public TipoRelatorioEnum Tipo { get; set; }
    public StatusRelatorioEnum Status { get; set; }
    public Dictionary<string, string> Conteudos { get; set; } = new();
    public DateTimeOffset? DataGeracao { get; set; }
}
