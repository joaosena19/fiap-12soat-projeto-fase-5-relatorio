namespace Application.Contracts.Gateways;

/// <summary>
/// Gateway para operações de persistência do resultado de diagrama.
/// </summary>
public interface IResultadoDiagramaGateway
{
    Task<Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama> SalvarAsync(Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama);
    Task<Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama?> ObterPorAnaliseDiagramaIdAsync(Guid analiseDiagramaId);
}