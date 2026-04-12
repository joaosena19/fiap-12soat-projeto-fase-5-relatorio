using Application.ResultadoDiagrama.UseCases;

namespace Tests.Helpers.Fixtures;

public class ListarResultadosDiagramaUseCaseTestFixture
{
    public Mock<IResultadoDiagramaGateway> GatewayMock { get; }
    public Mock<IListarResultadosDiagramaPresenter> PresenterMock { get; }
    public Mock<IAppLogger> LoggerMock { get; }
    public ListarResultadosDiagramaUseCase UseCase { get; }

    public ListarResultadosDiagramaUseCaseTestFixture()
    {
        GatewayMock = new Mock<IResultadoDiagramaGateway>();
        PresenterMock = new Mock<IListarResultadosDiagramaPresenter>();
        LoggerMock = MockLogger.Criar();
        UseCase = new ListarResultadosDiagramaUseCase();
    }

    public async Task ExecutarAsync()
    {
        await UseCase.ExecutarAsync(GatewayMock.Object, PresenterMock.Object, LoggerMock.Object);
    }
}
