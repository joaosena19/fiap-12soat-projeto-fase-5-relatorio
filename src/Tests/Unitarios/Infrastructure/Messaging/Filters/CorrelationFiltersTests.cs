using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Filters;

public class CorrelationFiltersTests
{
    [Fact(DisplayName = "Deve propagar correlation id no filtro de envio quando contexto já possui correlation")]
    [Trait("Infrastructure", "SendCorrelationIdFilter")]
    public async Task Send_SendFilter_DevePropagarCorrelationId_QuandoCorrelationContextPresente()
    {
        // Arrange
        var filtro = new SendCorrelationIdFilter<MensagemTeste>();
        var correlationIdEsperado = Guid.NewGuid().ToString();
        var correlationIdEsperadoGuid = Guid.Parse(correlationIdEsperado);
        var (contexto, headers) = SendContextMockExtensions.CriarSendContextComCorrelationId<MensagemTeste>();
        var pipe = new Mock<IPipe<SendContext<MensagemTeste>>>();
        pipe.AoEnviar(contexto.Object);

        // Act
        using (CorrelationContext.Push(correlationIdEsperado))
            await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.DeveTerSidoChamado(contexto.Object);
        contexto.Object.CorrelationId.ShouldBe(correlationIdEsperadoGuid);
        headers.DeveTerDefinidoHeader(CorrelationConstants.HeaderName, correlationIdEsperado);
    }

    [Fact(DisplayName = "Deve propagar correlation id no filtro de publicação quando contexto já possui correlation")]
    [Trait("Infrastructure", "PublishCorrelationIdFilter")]
    public async Task Send_PublishFilter_DevePropagarCorrelationId_QuandoCorrelationContextPresente()
    {
        // Arrange
        var filtro = new PublishCorrelationIdFilter<MensagemTeste>();
        var correlationIdEsperado = Guid.NewGuid().ToString();
        var correlationIdEsperadoGuid = Guid.Parse(correlationIdEsperado);
        var (contexto, headers) = SendContextMockExtensions.CriarPublishContextComCorrelationId<MensagemTeste>();
        var pipe = new Mock<IPipe<PublishContext<MensagemTeste>>>();
        pipe.AoEnviar(contexto.Object);

        // Act
        using (CorrelationContext.Push(correlationIdEsperado))
            await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.DeveTerSidoChamado(contexto.Object);
        contexto.Object.CorrelationId.ShouldBe(correlationIdEsperadoGuid);
        headers.DeveTerDefinidoHeader(CorrelationConstants.HeaderName, correlationIdEsperado);
    }

    [Fact(DisplayName = "Deve usar header HTTP quando header de correlação está presente")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_ConsumeFilter_DeveUsarHeader_QuandoHeaderCorrelacaoPresente()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var correlationIdEsperado = "header-correlation-id-valor";
        string? correlationIdCapturadoNoPipe = null;
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(new MensagemTeste());
        var pipe = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        headers.ComHeader(CorrelationConstants.HeaderName, correlationIdEsperado);
        pipe.AoEnviarCapturando(contexto.Object, () => correlationIdCapturadoNoPipe = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.DeveTerSidoChamado(contexto.Object);
        correlationIdCapturadoNoPipe.ShouldBe(correlationIdEsperado);
    }

    [Fact(DisplayName = "Deve usar propriedade CorrelationId da mensagem quando header e CorrelationId do contexto não estão disponíveis")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_ConsumeFilter_DeveUsarPropriedadeMensagem_QuandoHeaderECorrelationIdAusentes()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var mensagem = new MensagemTeste { CorrelationId = "correlation-da-propriedade" };
        string? correlationIdCapturadoNoPipe = null;
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(mensagem);
        var pipe = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        headers.SemHeader(CorrelationConstants.HeaderName);
        contexto.ComCorrelationId(null);
        pipe.AoEnviarCapturando(contexto.Object, () => correlationIdCapturadoNoPipe = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.DeveTerSidoChamado(contexto.Object);
        correlationIdCapturadoNoPipe.ShouldBe("correlation-da-propriedade");
    }

    [Fact(DisplayName = "Deve gerar novo GUID quando nenhuma fonte de correlação está disponível")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_ConsumeFilter_DeveGerarNovoGuid_QuandoNenhumaFonteDeCorrelacaoDisponivel()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var mensagem = new MensagemTeste { CorrelationId = null };
        string? correlationIdCapturadoNoPipe = null;
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(mensagem);
        var pipe = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        headers.SemHeader(CorrelationConstants.HeaderName);
        contexto.ComCorrelationId(null);
        pipe.AoEnviarCapturando(contexto.Object, () => correlationIdCapturadoNoPipe = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, pipe.Object);

        // Assert
        pipe.DeveTerSidoChamado(contexto.Object);
        correlationIdCapturadoNoPipe.ShouldNotBeNullOrWhiteSpace();
        Guid.TryParse(correlationIdCapturadoNoPipe, out _).ShouldBeTrue();
    }

    [Fact(DisplayName = "Probe deve criar escopo de filtros no filtro de envio")]
    [Trait("Infrastructure", "SendCorrelationIdFilter")]
    public void Probe_SendFilter_DeveCriarEscopoDeFiltros_QuandoInvocado()
    {
        // Arrange
        var filtro = new SendCorrelationIdFilter<MensagemTeste>();
        var probeContextMock = new TestProbeContext();

        // Act
        filtro.Probe(probeContextMock);

        // Assert
        probeContextMock.EscoposCriados.ShouldContain("filters");
    }

    [Fact(DisplayName = "Probe deve criar escopo de filtros no filtro de publicação")]
    [Trait("Infrastructure", "PublishCorrelationIdFilter")]
    public void Probe_PublishFilter_DeveCriarEscopoDeFiltros_QuandoInvocado()
    {
        // Arrange
        var filtro = new PublishCorrelationIdFilter<MensagemTeste>();
        var probeContextMock = new TestProbeContext();

        // Act
        filtro.Probe(probeContextMock);

        // Assert
        probeContextMock.EscoposCriados.ShouldContain("filters");
    }

    [Fact(DisplayName = "Probe deve criar escopo de filtros no filtro de consumo")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public void Probe_ConsumeFilter_DeveCriarEscopoDeFiltros_QuandoInvocado()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var probeContextMock = new TestProbeContext();

        // Act
        filtro.Probe(probeContextMock);

        // Assert
        probeContextMock.EscoposCriados.ShouldContain("filters");
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
