using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;

namespace Application.Contracts.Relatorios;

public interface IRelatorioStrategy
{
    TipoRelatorioEnum TipoRelatorio { get; }
    Task<ConteudosRelatorio> GerarAsync(Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama);
}