using Application.ResultadoDiagrama.UseCases;

namespace Tests.Helpers.Fixtures;

public class BuscarResultadoDiagramaPorIdUseCaseTestFixture
{
    public Mock<IResultadoDiagramaGateway> GatewayMock { get; }
    public Mock<IBuscarResultadoDiagramaPresenter> PresenterMock { get; }
    public Mock<IAppLogger> LoggerMock { get; }
    public BuscarResultadoDiagramaPorIdUseCase UseCase { get; }

    public BuscarResultadoDiagramaPorIdUseCaseTestFixture()
    {
        GatewayMock = new Mock<IResultadoDiagramaGateway>();
        PresenterMock = new Mock<IBuscarResultadoDiagramaPresenter>();
        LoggerMock = MockLogger.Criar();
        UseCase = new BuscarResultadoDiagramaPorIdUseCase();
    }

    public async Task ExecutarAsync(Guid analiseDiagramaId)
    {
        await UseCase.ExecutarAsync(analiseDiagramaId, GatewayMock.Object, PresenterMock.Object, LoggerMock.Object);
    }
}