using Domain.ResultadoDiagrama.Enums;

namespace Application.Contracts.Relatorios;

/// <summary>
/// Resolve a strategy de geração de relatório para um dado tipo.
/// </summary>
public interface IRelatorioStrategyResolver
{
    IRelatorioStrategy Resolver(TipoRelatorioEnum tipoRelatorio);
}
