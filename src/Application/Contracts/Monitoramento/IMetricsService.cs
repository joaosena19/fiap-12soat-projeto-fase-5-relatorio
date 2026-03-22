using Domain.AnaliseDiagrama.Enums;

namespace Application.Contracts.Monitoramento;

public interface IMetricsService
{
    void RegistrarAnaliseRecebida(Guid analiseDiagramaId, string extensao);
    void RegistrarAnaliseConcluida(Guid analiseDiagramaId);
    void RegistrarAnaliseComFalha(Guid analiseDiagramaId, string motivo);
    void RegistrarRelatorioGerado(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio);
    void RegistrarRelatorioComFalha(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio, string motivo);
}
