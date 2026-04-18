using Application.Contracts.Messaging.Dtos;
using Infrastructure.Messaging.Publishers;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Publishers;

public class RelatorioMessagePublisherTests
{
    private readonly RelatorioMessagePublisherTestFixture _fixture = new();

    [Fact(DisplayName = "Deve publicar mensagem com CorrelationId e dados corretos")]
    [Trait("Infrastructure", "RelatorioMessagePublisher")]
    public async Task PublicarSolicitacaoGeracaoAsync_DevePublicarMensagem()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json, TipoRelatorioEnum.Markdown };
        var correlationId = Guid.NewGuid().ToString();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId);

        // Act
        await _fixture.Publisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<SolicitarGeracaoRelatoriosDto>();
        mensagem.CorrelationId.ShouldBe(correlationId);
        mensagem.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        mensagem.TiposRelatorio.ShouldBe(tiposRelatorio);
    }

    [Fact(DisplayName = "Deve propagar exceção e logar erro quando publicação falhar")]
    [Trait("Infrastructure", "RelatorioMessagePublisher")]
    public async Task PublicarSolicitacaoGeracaoAsync_DeveLancarExcecao_QuandoPublicacaoFalhar()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new List<TipoRelatorioEnum> { TipoRelatorioEnum.Json };
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(Guid.NewGuid().ToString());
        _fixture.PublishEndpointMock.Setup(x => x.Publish(It.IsAny<SolicitarGeracaoRelatoriosDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Falha na conexão"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() => _fixture.Publisher.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, tiposRelatorio));
    }

}
