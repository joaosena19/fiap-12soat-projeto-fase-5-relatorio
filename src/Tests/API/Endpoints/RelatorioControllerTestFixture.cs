using API.Endpoints;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.API.Endpoints;

public class RelatorioControllerTestFixture : IDisposable
{
    public AppDbContext Contexto { get; }
    public Mock<IRelatorioMessagePublisher> MessagePublisherMock { get; } = new();
    public RelatorioController Controller { get; }

    public RelatorioControllerTestFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"relatorio-controller-{Guid.NewGuid()}")
            .Options;

        Contexto = new AppDbContext(options);
        var loggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();
        Controller = new RelatorioController(Contexto, loggerFactoryMock.Object, MessagePublisherMock.Object);
    }

    public void Dispose() => Contexto.Dispose();
}
