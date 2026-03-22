namespace Application.Contracts.Monitoramento;

public interface IMetricsService
{
    void RegistrarAnaliseRecebida(Guid analiseDiagramaId, string extensao);
    void RegistrarAnaliseConcluida(Guid analiseDiagramaId);
    void RegistrarAnaliseComFalha(Guid analiseDiagramaId, string motivo);
}
