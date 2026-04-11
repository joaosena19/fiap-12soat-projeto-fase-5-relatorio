using Domain.ResultadoDiagrama.Enums;

namespace Application.Contracts.Messaging;

public interface IRelatorioMessagePublisher
{
    Task PublicarSolicitacaoGeracaoAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio);
}