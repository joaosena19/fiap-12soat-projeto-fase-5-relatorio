using Domain.ResultadoDiagrama.Enums;

namespace API.Dtos;

public class SolicitarRelatoriosRequestDto
{
    public List<TipoRelatorioEnum> TiposRelatorio { get; set; } = new();
}