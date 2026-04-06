using MassTransit;

namespace Tests.Helpers.MockExtensions;

public static class PublishEndpointMockExtensions
{
    public static void DeveTerPublicado<T>(this Mock<IPublishEndpoint> mock) where T : class
    {
        mock.Verify(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    public static T ObterMensagemPublicada<T>(this Mock<IPublishEndpoint> mock, int indice = 0) where T : class
    {
        mock.Invocations.Count.ShouldBeGreaterThan(indice, $"Esperava ao menos {indice + 1} publicação(ões), mas encontrou {mock.Invocations.Count}");
        return mock.Invocations[indice].Arguments[0].ShouldBeOfType<T>();
    }

    public static async Task<Guid?> ObterCorrelationIdDaPipe<T>(this Mock<IPublishEndpoint> mock, int indice = 0) where T : class
    {
        var pipe = mock.Invocations[indice].Arguments[1] as IPipe<PublishContext<T>>;
        pipe.ShouldNotBeNull();
        var contextoPublicacao = new Mock<PublishContext<T>>();
        contextoPublicacao.SetupProperty(item => item.CorrelationId);
        await pipe!.Send(contextoPublicacao.Object);
        return contextoPublicacao.Object.CorrelationId;
    }
}
