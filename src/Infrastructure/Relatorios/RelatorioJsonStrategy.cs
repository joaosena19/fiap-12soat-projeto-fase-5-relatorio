using Application.Contracts.Relatorios;
using Domain.AnaliseDiagrama.Aggregates;
using Domain.AnaliseDiagrama.Enums;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Shared.Constants;
using System.Text.Json;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato JSON a partir da análise do diagrama.
/// </summary>
public class RelatorioJsonStrategy : IRelatorioStrategy
{
    public TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Json;

    public Task<ConteudosRelatorio> GerarAsync(ResultadoDiagrama resultadoDiagrama)
    {
        var analise = resultadoDiagrama.AnaliseResultado ?? throw new InvalidOperationException("Análise não está disponível para gerar JSON");

        var jsonString = JsonSerializer.Serialize(new
        {
            DescricaoAnalise = analise.DescricaoAnalise.Valor,
            ComponentesIdentificados = analise.ComponentesIdentificados.Select(item => item.Valor).ToList(),
            RiscosArquiteturais = analise.RiscosArquiteturais.Select(item => item.Valor).ToList(),
            RecomendacoesBasicas = analise.RecomendacoesBasicas.Select(item => item.Valor).ToList()
        });

        var conteudos = ConteudosRelatorio.Vazio().Adicionar(ConteudoRelatorioChaves.JsonString, jsonString);

        return Task.FromResult(conteudos);
    }
}
