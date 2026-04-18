using Domain.ResultadoDiagrama.Enums;

namespace Application.Contracts.Messaging.Dtos;

public record SolicitarGeracaoRelatoriosDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public Guid AnaliseDiagramaId { get; init; }
    public List<TipoRelatorioEnum> TiposRelatorio { get; init; } = new();
}