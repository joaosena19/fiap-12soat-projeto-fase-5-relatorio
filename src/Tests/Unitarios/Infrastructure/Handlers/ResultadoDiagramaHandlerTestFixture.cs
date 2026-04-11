using Infrastructure.Handlers;

namespace Tests.Infrastructure.Handlers;

public class ResultadoDiagramaHandlerTestFixture
{
    public ResultadoDiagramaHandler Handler { get; }
    public Mock<IResultadoDiagramaGateway> GatewayMock { get; } = new();
    public Mock<IBuscarResultadoDiagramaPresenter> BuscarPresenterMock { get; } = new();
    public Mock<ISolicitarGeracaoRelatoriosPresenter> SolicitarPresenterMock { get; } = new();
    public Mock<IRelatorioMessagePublisher> MessagePublisherMock { get; } = new();

    public ResultadoDiagramaHandlerTestFixture()
    {
        var loggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();
        Handler = new ResultadoDiagramaHandler(loggerFactoryMock.Object);
    }

    public async Task BuscarPorAnaliseDiagramaIdAsync(Guid analiseDiagramaId) => await Handler.BuscarPorAnaliseDiagramaIdAsync(analiseDiagramaId, GatewayMock.Object, BuscarPresenterMock.Object);

    public async Task SolicitarGeracaoRelatoriosAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio) => await Handler.SolicitarGeracaoRelatoriosAsync(analiseDiagramaId, tiposRelatorio, GatewayMock.Object, MessagePublisherMock.Object, SolicitarPresenterMock.Object);
}
