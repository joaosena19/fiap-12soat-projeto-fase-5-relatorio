using Application.Contracts.Gateways;
using Application.Contracts.Messaging;
using Application.Contracts.Presenters;
using Application.ResultadoDiagrama.UseCases;
using Domain.ResultadoDiagrama.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers;

public class ResultadoDiagramaHandler : BaseHandler
{
    public ResultadoDiagramaHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    public async Task ListarResultadosDiagramaAsync(IResultadoDiagramaGateway gateway, IListarResultadosDiagramaPresenter presenter)
    {
        var useCase = new ListarResultadosDiagramaUseCase();
        var logger = CriarLoggerPara<ListarResultadosDiagramaUseCase>();

        await useCase.ExecutarAsync(gateway, presenter, logger);
    }

    public async Task BuscarPorAnaliseDiagramaIdAsync(Guid analiseDiagramaId, IResultadoDiagramaGateway gateway, IBuscarResultadoDiagramaPresenter presenter)
    {
        var useCase = new BuscarResultadoDiagramaPorIdUseCase();
        var logger = CriarLoggerPara<BuscarResultadoDiagramaPorIdUseCase>();

        await useCase.ExecutarAsync(analiseDiagramaId, gateway, presenter, logger);
    }

    public async Task SolicitarGeracaoRelatoriosAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio, IResultadoDiagramaGateway gateway, IRelatorioMessagePublisher messagePublisher, ISolicitarGeracaoRelatoriosPresenter presenter)
    {
        var useCase = new SolicitarGeracaoRelatoriosUseCase();
        var logger = CriarLoggerPara<SolicitarGeracaoRelatoriosUseCase>();

        await useCase.ExecutarAsync(analiseDiagramaId, tiposRelatorio, gateway, messagePublisher, presenter, logger);
    }
}