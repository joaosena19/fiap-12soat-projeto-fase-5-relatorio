using Domain.ResultadoDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class RelatorioResumoDto
{
    public TipoRelatorioEnum Tipo { get; set; }
    public StatusRelatorioEnum Status { get; set; }
}
