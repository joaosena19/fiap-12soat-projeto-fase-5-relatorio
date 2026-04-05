using MassTransit;

namespace Tests.Helpers.MockExtensions;

public static class SendContextMockExtensions
{
    public static (Mock<SendContext<T>> sendContextMock, Mock<SendHeaders> headersMock) CriarSendContext<T>() where T : class
    {
        var headersMock = new Mock<SendHeaders>();
        var sendContextMock = new Mock<SendContext<T>>();
        sendContextMock.Setup(x => x.Headers).Returns(headersMock.Object);
        return (sendContextMock, headersMock);
    }
}
