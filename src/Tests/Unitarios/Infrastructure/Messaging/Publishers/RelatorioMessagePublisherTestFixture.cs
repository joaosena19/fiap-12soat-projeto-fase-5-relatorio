using Infrastructure.Messaging.Publishers;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Publishers;

public class RelatorioMessagePublisherTestFixture
{
    public Mock<IPublishEndpoint> PublishEndpointMock { get; } = new();
    public Mock<ICorrelationIdAccessor> CorrelationIdAccessorMock { get; } = new();
    public RelatorioMessagePublisher Publisher { get; }

    public RelatorioMessagePublisherTestFixture()
    {
        var loggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();
        Publisher = new RelatorioMessagePublisher(PublishEndpointMock.Object, CorrelationIdAccessorMock.Object, loggerFactoryMock.Object);
    }
}
