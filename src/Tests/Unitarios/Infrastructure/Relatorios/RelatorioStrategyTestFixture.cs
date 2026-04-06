using Application.Contracts.Armazenamento;
using Infrastructure.Relatorios;

namespace Tests.Infrastructure.Relatorios;

public class RelatorioStrategyTestFixture<TStrategy> where TStrategy : class, IRelatorioStrategy
{
    public Mock<IArmazenamentoArquivoService> ArmazenamentoMock { get; } = new();
    public TStrategy Strategy { get; }

    public RelatorioStrategyTestFixture(Func<IArmazenamentoArquivoService, Microsoft.Extensions.Logging.ILoggerFactory, TStrategy> factory)
    {
        var loggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();
        Strategy = factory(ArmazenamentoMock.Object, loggerFactoryMock.Object);
    }
}
