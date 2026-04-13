using Domain.ResultadoDiagrama.Enums;

namespace Application.Contracts.Monitoramento;

public interface IMetricsService
{
    void RegistrarAnaliseRecebida(Guid analiseDiagramaId, string extensao);
    void RegistrarAnaliseConcluida(Guid analiseDiagramaId);
    void RegistrarFalhaProcessamentoRecebida(Guid analiseDiagramaId, string motivo, string? origemErro, int tentativasRealizadas);
    void RegistrarRejeicaoUploadRecebida(Guid analiseDiagramaId, string motivoRejeicao);
    void RegistrarRelatorioGerado(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio);
    void RegistrarRelatorioComFalha(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio, string motivo);
}
