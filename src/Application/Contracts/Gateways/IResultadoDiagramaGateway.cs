namespace Application.Contracts.Gateways;

/// <summary>
/// Gateway para operações de persistência do resultado de diagrama.
/// </summary>
public interface IResultadoDiagramaGateway
{
    Task<Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama> SalvarAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama);
    Task<Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama?> ObterPorAnaliseDiagramaIdAsync(Guid analiseDiagramaId);
}