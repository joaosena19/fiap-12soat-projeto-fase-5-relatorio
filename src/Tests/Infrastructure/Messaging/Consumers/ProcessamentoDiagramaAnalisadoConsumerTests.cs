using Microsoft.EntityFrameworkCore;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class ProcessamentoDiagramaAnalisadoConsumerTests
{
    [Fact(DisplayName = "Deve registrar análise e publicar solicitação quando resultado existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaAnalisadoConsumer")]
    public async Task Consume_DeveRegistrarAnaliseEPublicar_QuandoResultadoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerAnalisado();
        var mensagem = new ProcessamentoDiagramaAnalisadoDtoBuilder().ComAnaliseDiagramaId(analiseDiagramaId).ComDescricaoAnalise("Análise arquitetural completa").Build();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoAnalisado(mensagem, Guid.NewGuid());

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarAnalisadoComDescricao("Análise arquitetural completa");
        fixture.DeveTerPublicadoSolicitacaoGeracao(analiseDiagramaId);
    }

    [Fact(DisplayName = "Não deve publicar quando resultado não existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaAnalisadoConsumer")]
    public async Task Consume_NaoDevePublicar_QuandoResultadoNaoExistir()
    {
        // Arrange
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = fixture.CriarConsumerAnalisado();
        var mensagem = new ProcessamentoDiagramaAnalisadoDtoBuilder().ComAnaliseDiagramaId(Guid.NewGuid()).Build();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoAnalisado(mensagem);

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        fixture.NaoDeveTerPublicadoSolicitacaoGeracao();
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto falha")]
    [Trait("Infrastructure", "ProcessamentoDiagramaAnalisadoConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoFalha()
    {
        // Arrange
        var consumer = ResultadoDiagramaConsumerTestFixture.CriarConsumerAnalisadoComContextoDescartado();
        var mensagem = new ProcessamentoDiagramaAnalisadoDtoBuilder().Build();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoAnalisado(mensagem);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}