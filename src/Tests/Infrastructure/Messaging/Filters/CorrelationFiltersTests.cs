using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Filters;

public class CorrelationFiltersTests
{
    [Fact(DisplayName = "Deve executar próximo pipe no filtro de envio")]
    [Trait("Infrastructure", "SendCorrelationIdFilter")]
    public async Task Send_SendFilter_DeveExecutarProximoPipe_QuandoInvocado()
    {
        // Arrange
        var filtro = new SendCorrelationIdFilter<MensagemTeste>();
        var contexto = new Mock<SendContext<MensagemTeste>>();
        var headers = new Mock<SendHeaders>();
        var pipe = new Mock<IPipe<SendContext<MensagemTeste>>>();

        contexto.Setup(x => x.Headers).Returns(headers.Object);

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Deve executar próximo pipe no filtro de publicação")]
    [Trait("Infrastructure", "PublishCorrelationIdFilter")]
    public async Task Send_PublishFilter_DeveExecutarProximoPipe_QuandoInvocado()
    {
        // Arrange
        var filtro = new PublishCorrelationIdFilter<MensagemTeste>();
        var contexto = new Mock<PublishContext<MensagemTeste>>();
        var headers = new Mock<SendHeaders>();
        var pipe = new Mock<IPipe<PublishContext<MensagemTeste>>>();

        contexto.Setup(x => x.Headers).Returns(headers.Object);

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Deve executar próximo pipe no filtro de consumo")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_ConsumeFilter_DeveExecutarProximoPipe_QuandoInvocado()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var headers = new Mock<Headers>();
        var pipe = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();

        object? valorHeader;
        headers.Setup(x => x.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(false);
        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.Setup(x => x.CorrelationId).Returns(Guid.NewGuid());
        contexto.Setup(x => x.Message).Returns(new MensagemTeste());

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Deve usar header HTTP quando header de correlação está presente")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_ConsumeFilter_DeveUsarHeader_QuandoHeaderCorrelacaoPresente()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var headers = new Mock<Headers>();
        var pipe = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        var correlationIdEsperado = "header-correlation-id-valor";

        object? valorHeader = correlationIdEsperado;
        headers.Setup(x => x.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(true);
        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.Setup(x => x.Message).Returns(new MensagemTeste());

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Deve usar propriedade CorrelationId da mensagem quando header e CorrelationId do contexto não estão disponíveis")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_ConsumeFilter_DeveUsarPropriedadeMensagem_QuandoHeaderECorrelationIdAusentes()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var headers = new Mock<Headers>();
        var pipe = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        var mensagem = new MensagemTeste { CorrelationId = "correlation-da-propriedade" };

        object? valorHeader = null;
        headers.Setup(x => x.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(false);
        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.Setup(x => x.CorrelationId).Returns((Guid?)null);
        contexto.Setup(x => x.Message).Returns(mensagem);

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Deve gerar novo GUID quando nenhuma fonte de correlação está disponível")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_ConsumeFilter_DeveGerarNovoGuid_QuandoNenhumaFonteDeCorrelacaoDisponivel()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var headers = new Mock<Headers>();
        var pipe = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        var mensagem = new MensagemTeste { CorrelationId = null };

        object? valorHeader = null;
        headers.Setup(x => x.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(false);
        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.Setup(x => x.CorrelationId).Returns((Guid?)null);
        contexto.Setup(x => x.Message).Returns(mensagem);

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Probe deve executar sem exceção no filtro de envio")]
    [Trait("Infrastructure", "SendCorrelationIdFilter")]
    public void Probe_SendFilter_DeveExecutarSemExcecao_QuandoInvocado()
    {
        // Arrange
        var filtro = new SendCorrelationIdFilter<MensagemTeste>();
        var probeContextMock = new TestProbeContext();

        // Act & Assert
        Should.NotThrow(() => filtro.Probe(probeContextMock));
        probeContextMock.EscoposCriados.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "Probe deve executar sem exceção no filtro de publicação")]
    [Trait("Infrastructure", "PublishCorrelationIdFilter")]
    public void Probe_PublishFilter_DeveExecutarSemExcecao_QuandoInvocado()
    {
        // Arrange
        var filtro = new PublishCorrelationIdFilter<MensagemTeste>();
        var probeContextMock = new TestProbeContext();

        // Act & Assert
        Should.NotThrow(() => filtro.Probe(probeContextMock));
        probeContextMock.EscoposCriados.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "Probe deve executar sem exceção no filtro de consumo")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public void Probe_ConsumeFilter_DeveExecutarSemExcecao_QuandoInvocado()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var probeContextMock = new TestProbeContext();

        // Act & Assert
        Should.NotThrow(() => filtro.Probe(probeContextMock));
        probeContextMock.EscoposCriados.ShouldNotBeEmpty();
    }

    public class MensagemTeste
    {
        public string? CorrelationId { get; set; }
    }

    private sealed class TestProbeContext : ProbeContext
    {
        public List<string> EscoposCriados { get; } = new();

        public CancellationToken CancellationToken => CancellationToken.None;

        public ProbeContext CreateScope(string name)
        {
            EscoposCriados.Add(name);
            return this;
        }

        public void Add(string key, string value) { }
        public void Add(string key, object value) { }
        public void Set(object values) { }
        public void Set(IEnumerable<KeyValuePair<string, object>> values) { }
    }
}
