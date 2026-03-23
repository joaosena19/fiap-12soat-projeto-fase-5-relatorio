namespace Application.Contracts.Messaging.Dtos;

public record UploadDiagramaRejeitadoDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public Guid AnaliseDiagramaId { get; init; }
    public string MotivoRejeicao { get; init; } = string.Empty;
    public DateTimeOffset DataRejeicao { get; init; }
}