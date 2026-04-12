using Domain.ResultadoDiagrama.Enums;

namespace Domain.ResultadoDiagrama.Constants;

/// <summary>
/// Tipos de relatório que são gerados automaticamente quando a análise conclui.
/// </summary>
public static class TiposRelatorioAutomatico
{
    public static readonly HashSet<TipoRelatorioEnum> Tipos = [TipoRelatorioEnum.Json];
}
