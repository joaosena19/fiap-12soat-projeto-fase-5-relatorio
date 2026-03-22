using Application.Contracts.Monitoramento;
using Domain.ResultadoDiagrama.Enums;
using Shared.Constants;
using Infrastructure.Monitoramento.Correlation;
using NR = NewRelic.Api.Agent;

namespace Infrastructure.Monitoramento;

/// <summary>
/// Implementação de métricas customizadas usando New Relic.
/// </summary>
public class NewRelicMetricsService : IMetricsService
{
    public void RegistrarAnaliseRecebida(Guid analiseDiagramaId, string extensao)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Extensao, extensao },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("AnaliseDiagramaRecebida", atributos);
    }

    public void RegistrarAnaliseConcluida(Guid analiseDiagramaId)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("AnaliseDiagramaConcluida", atributos);
    }

    public void RegistrarAnaliseComFalha(Guid analiseDiagramaId, string motivo)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Motivo, motivo },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("AnaliseDiagramaComFalha", atributos);
    }

    public void RegistrarRelatorioGerado(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.TipoRelatorio, tipoRelatorio.ToString() },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("RelatorioGerado", atributos);
    }

    public void RegistrarRelatorioComFalha(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio, string motivo)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.TipoRelatorio, tipoRelatorio.ToString() },
            { LogNomesPropriedades.Motivo, motivo },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("RelatorioComFalha", atributos);
    }

    private static void AdicionarCorrelationId(Dictionary<string, object> atributos)
    {
        var correlationId = CorrelationContext.Current;
        if (!string.IsNullOrWhiteSpace(correlationId))
            atributos[LogNomesPropriedades.CorrelationId] = correlationId;
    }
}
