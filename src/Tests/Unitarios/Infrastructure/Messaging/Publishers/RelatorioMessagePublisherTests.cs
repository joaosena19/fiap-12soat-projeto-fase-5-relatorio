using Application.Contracts.Messaging.Dtos;
using Infrastructure.Messaging.Publishers;
using MassTransit;

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
        var correlationId = Guid.NewGuid();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId.ToString());

        // Act
        await _fixture.Publisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<SolicitarGeracaoRelatoriosDto>();
        mensagem.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        mensagem.TiposRelatorio.ShouldBe(tiposRelatorio);
        var correlationIdObtido = await _fixture.PublishEndpointMock.ObterCorrelationIdDaPipe<SolicitarGeracaoRelatoriosDto>();
        correlationIdObtido.ShouldBe(correlationId);
    }

    [Fact(DisplayName = "Deve publicar mesmo quando correlation ID não for GUID válido")]
    [Trait("Infrastructure", "RelatorioMessagePublisher")]
    public async Task PublicarSolicitacaoGeracaoAsync_DevePublicar_QuandoCorrelationIdInvalido()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json };
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId("correlation-id-invalido");

        // Act
        await _fixture.Publisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<SolicitarGeracaoRelatoriosDto>();
        mensagem.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        mensagem.TiposRelatorio.ShouldBe(tiposRelatorio);
        var correlationIdObtido = await _fixture.PublishEndpointMock.ObterCorrelationIdDaPipe<SolicitarGeracaoRelatoriosDto>();
        correlationIdObtido.ShouldBeNull();
    }

}
