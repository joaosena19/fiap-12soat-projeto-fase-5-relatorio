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
    private const string EventoFalhaProcessamentoRecebida = "FalhaProcessamentoRecebida";
    private const string EventoRejeicaoUploadRecebida = "RejeicaoUploadRecebida";
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

    public void RegistrarFalhaProcessamentoRecebida(Guid analiseDiagramaId, string motivo, string? origemErro, int tentativasRealizadas)
    {
        RegistrarEvento(EventoFalhaProcessamentoRecebida, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Motivo, motivo },
            { LogNomesPropriedades.OrigemErro, origemErro ?? "Desconhecido" },
            { LogNomesPropriedades.Tentativas, tentativasRealizadas }
        });
    }

    public void RegistrarRejeicaoUploadRecebida(Guid analiseDiagramaId, string motivoRejeicao)
    {
        RegistrarEvento(EventoRejeicaoUploadRecebida, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Motivo, motivoRejeicao }
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
        AdicionarAppName(atributos);
        NR.NewRelic.RecordCustomEvent(nomeEvento, atributos);
    }

    private static void AdicionarCorrelationId(Dictionary<string, object> atributos)
    {
        var correlationId = CorrelationContext.Current;
        if (!string.IsNullOrWhiteSpace(correlationId))
            atributos[LogNomesPropriedades.CorrelationId] = correlationId;
    }

    private static void AdicionarAppName(Dictionary<string, object> atributos)
    {
        var appName = Environment.GetEnvironmentVariable("NEW_RELIC_APP_NAME");
        if (!string.IsNullOrWhiteSpace(appName))
            atributos[LogNomesPropriedades.AppName] = appName;
    }
}
