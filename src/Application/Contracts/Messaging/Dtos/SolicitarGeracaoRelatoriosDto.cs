using Domain.ResultadoDiagrama.Enums;

namespace Application.Contracts.Messaging.Dtos;

public record SolicitarGeracaoRelatoriosDto
{
    public Guid AnaliseDiagramaId { get; init; }
    public List<TipoRelatorioEnum> TiposRelatorio { get; init; } = new();
}