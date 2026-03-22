using Domain.AnaliseDiagrama.Enums;

namespace Application.Contracts.Messaging.Dtos;

public record SolicitarGeracaoRelatoriosDto
{
    public Guid AnaliseDiagramaId { get; init; }
    public List<TipoRelatorioEnum> TiposRelatorio { get; init; } = new();
}