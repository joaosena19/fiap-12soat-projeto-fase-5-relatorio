using Infrastructure.Messaging.Publishers;

namespace Tests.Infrastructure.Messaging.Publishers;

public class RelatorioMessagePublisherTests
{
    private readonly RelatorioMessagePublisherTestFixture _fixture = new();

    [Fact(DisplayName = "Deve publicar mensagem via IPublishEndpoint")]
    [Trait("Infrastructure", "RelatorioMessagePublisher")]
    public async Task PublicarSolicitacaoGeracaoAsync_DevePublicarMensagem()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json, TipoRelatorioEnum.Markdown };
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId();

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposRelatorio));
    }

    [Fact(DisplayName = "Deve publicar mesmo quando correlation ID não for GUID válido")]
    [Trait("Infrastructure", "RelatorioMessagePublisher")]
    public async Task PublicarSolicitacaoGeracaoAsync_DevePublicar_QuandoCorrelationIdInvalido()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json };
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId("correlation-id-invalido");

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposRelatorio));
    }

}
