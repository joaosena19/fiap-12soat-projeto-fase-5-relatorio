using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using System.Text.Json;

namespace Infrastructure.Relatorios;

/// <summary>
/// Estratégia de geração de relatório em formato JSON a partir da análise do diagrama.
/// </summary>
public class RelatorioJsonStrategy : BaseRelatorioStrategy
{
    public RelatorioJsonStrategy(ILoggerFactory loggerFactory) : base(loggerFactory.CriarAppLogger<RelatorioJsonStrategy>()) { }

    public override TipoRelatorioEnum TipoRelatorio => TipoRelatorioEnum.Json;

    protected override Task<ConteudosRelatorio> GerarConteudoAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama, AnaliseResultado analise)
    {
        try
        {
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
        catch (Exception ex)
        {
            CriarLoggerContextualizado(resultadoDiagrama).LogError(ex, $"Erro ao gerar conteúdo do relatório {{{LogNomesPropriedades.TipoRelatorio}}} para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", TipoRelatorio, resultadoDiagrama.AnaliseDiagramaId);
            throw;
        }
    }
}
