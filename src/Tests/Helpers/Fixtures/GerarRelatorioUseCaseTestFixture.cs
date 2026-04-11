using Application.ResultadoDiagrama.UseCases;

namespace Tests.Helpers.Fixtures;

public class GerarRelatorioUseCaseTestFixture
{
    public Mock<IResultadoDiagramaGateway> GatewayMock { get; }
    public Mock<IRelatorioStrategyResolver> StrategyResolverMock { get; }
    public Mock<IRelatorioStrategy> StrategyMock { get; }
    public Mock<IMetricsService> MetricsMock { get; }
    public Mock<IAppLogger> LoggerMock { get; }
    public GerarRelatorioUseCase UseCase { get; }

    public GerarRelatorioUseCaseTestFixture()
    {
        GatewayMock = new Mock<IResultadoDiagramaGateway>();
        StrategyResolverMock = new Mock<IRelatorioStrategyResolver>();
        StrategyMock = new Mock<IRelatorioStrategy>();
        MetricsMock = new Mock<IMetricsService>();
        LoggerMock = MockLogger.Criar();
        UseCase = new GerarRelatorioUseCase();

        StrategyMock.SetupGet(x => x.TipoRelatorio).Returns(TipoRelatorioEnum.Markdown);
        StrategyResolverMock.AoResolver(TipoRelatorioEnum.Markdown).Retorna(StrategyMock.Object);
        GatewayMock.AoSalvar().RetornaMesmoObjeto();
    }

    public async Task ExecutarAsync(Guid analiseDiagramaId, TipoRelatorioEnum tipoRelatorio = TipoRelatorioEnum.Markdown)
    {
        await UseCase.ExecutarAsync(analiseDiagramaId, tipoRelatorio, GatewayMock.Object, StrategyResolverMock.Object, MetricsMock.Object, LoggerMock.Object);
    }
}