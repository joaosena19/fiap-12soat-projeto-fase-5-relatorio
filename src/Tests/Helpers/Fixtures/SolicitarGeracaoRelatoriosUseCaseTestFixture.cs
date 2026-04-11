using Application.ResultadoDiagrama.UseCases;

namespace Tests.Helpers.Fixtures;

public class SolicitarGeracaoRelatoriosUseCaseTestFixture
{
    public Mock<IResultadoDiagramaGateway> GatewayMock { get; }
    public Mock<IRelatorioMessagePublisher> MessagePublisherMock { get; }
    public Mock<ISolicitarGeracaoRelatoriosPresenter> PresenterMock { get; }
    public Mock<IAppLogger> LoggerMock { get; }
    public SolicitarGeracaoRelatoriosUseCase UseCase { get; }

    public SolicitarGeracaoRelatoriosUseCaseTestFixture()
    {
        GatewayMock = new Mock<IResultadoDiagramaGateway>();
        MessagePublisherMock = new Mock<IRelatorioMessagePublisher>();
        PresenterMock = new Mock<ISolicitarGeracaoRelatoriosPresenter>();
        LoggerMock = MockLogger.Criar();
        UseCase = new SolicitarGeracaoRelatoriosUseCase();
    }

    public async Task ExecutarAsync(Guid analiseDiagramaId, IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio)
    {
        await UseCase.ExecutarAsync(analiseDiagramaId, tiposRelatorio, GatewayMock.Object, MessagePublisherMock.Object, PresenterMock.Object, LoggerMock.Object);
    }
}