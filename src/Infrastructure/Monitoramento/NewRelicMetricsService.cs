using Application.Contracts.Monitoramento;
using Shared.Constants;
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
        NR.NewRelic.RecordCustomEvent("AnaliseDiagramaRecebida", atributos);
    }

    public void RegistrarAnaliseConcluida(Guid analiseDiagramaId)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };
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
        NR.NewRelic.RecordCustomEvent("AnaliseDiagramaComFalha", atributos);
    }
}
