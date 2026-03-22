using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Enums;

namespace Infrastructure.Relatorios;

/// <summary>
/// Resolve a strategy de geração de relatório a partir das implementações registradas no DI.
/// </summary>
public class RelatorioStrategyResolver : IRelatorioStrategyResolver
{
    private readonly IEnumerable<IRelatorioStrategy> _strategies;

    public RelatorioStrategyResolver(IEnumerable<IRelatorioStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IRelatorioStrategy Resolver(TipoRelatorioEnum tipoRelatorio)
    {
        return _strategies.FirstOrDefault(item => item.TipoRelatorio == tipoRelatorio)
            ?? throw new InvalidOperationException($"Strategy de relatório não encontrada para o tipo '{tipoRelatorio}'");
    }
}
