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
    private const string EventoAnaliseRecebida = "AnaliseDiagramaRecebida";
    private const string EventoAnaliseConcluida = "AnaliseDiagramaConcluida";
    private const string EventoAnaliseComFalha = "AnaliseDiagramaComFalha";
    private const string EventoRelatorioGerado = "RelatorioGerado";
    private const string EventoRelatorioComFalha = "RelatorioComFalha";

    public void RegistrarAnaliseRecebida(Guid analiseDiagramaId, string extensao)
    {
        RegistrarEvento(EventoAnaliseRecebida, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Extensao, extensao }
        });
    }

    public void RegistrarAnaliseConcluida(Guid analiseDiagramaId)
    {
        RegistrarEvento(EventoAnaliseConcluida, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId }
        });
    }

    public void RegistrarAnaliseComFalha(Guid analiseDiagramaId, string motivo)
    {
        RegistrarEvento(EventoAnaliseComFalha, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Motivo, motivo }
        });
    }

    public void RegistrarRelatorioGerado(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio)
    {
        RegistrarEvento(EventoRelatorioGerado, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.TipoRelatorio, tipoRelatorio.ToString() }
        });
    }

    public void RegistrarRelatorioComFalha(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio, string motivo)
    {
        RegistrarEvento(EventoRelatorioComFalha, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.TipoRelatorio, tipoRelatorio.ToString() },
            { LogNomesPropriedades.Motivo, motivo }
        });
    }

    private static void RegistrarEvento(string nomeEvento, Dictionary<string, object> atributos)
    {
        atributos[LogNomesPropriedades.Timestamp] = DateTimeOffset.UtcNow;
        AdicionarCorrelationId(atributos);
        NR.NewRelic.RecordCustomEvent(nomeEvento, atributos);
    }

    private static void AdicionarCorrelationId(Dictionary<string, object> atributos)
    {
        var correlationId = CorrelationContext.Current;
        if (!string.IsNullOrWhiteSpace(correlationId))
            atributos[LogNomesPropriedades.CorrelationId] = correlationId;
    }
}
