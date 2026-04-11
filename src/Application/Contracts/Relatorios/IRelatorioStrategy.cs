using Domain.ResultadoDiagrama.Enums;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;

namespace Application.Contracts.Relatorios;

public interface IRelatorioStrategy
{
    TipoRelatorioEnum TipoRelatorio { get; }
    Task<ConteudosRelatorio> GerarAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama);
}