using MassTransit;

namespace Tests.Helpers.MockExtensions;

public static class PublishEndpointMockExtensions
{
    public static void DeveTerPublicado<T>(this Mock<IPublishEndpoint> mock) where T : class
    {
        mock.Verify(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
