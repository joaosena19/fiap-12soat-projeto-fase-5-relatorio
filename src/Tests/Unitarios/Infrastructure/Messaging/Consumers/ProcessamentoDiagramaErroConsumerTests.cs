using Microsoft.EntityFrameworkCore;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class ProcessamentoDiagramaErroConsumerTests
{
    [Fact(DisplayName = "Deve registrar falha de processamento quando resultado existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaErroConsumer")]
    public async Task Consume_DeveRegistrarFalha_QuandoResultadoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerErro();
        var mensagem = new ProcessamentoDiagramaErroDtoBuilder().ComAnaliseDiagramaId(analiseDiagramaId).ComMotivo("Timeout no processamento de LLM").ComTentativas(3).Build();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoErro(mensagem, Guid.NewGuid());

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveConterErroComMensagem("Timeout no processamento de LLM");
        resultado.DeveEstarComErro();
    }

    [Fact(DisplayName = "Deve registrar rejeição quando mensagem indicar rejeição")]
    [Trait("Infrastructure", "ProcessamentoDiagramaErroConsumer")]
    public async Task Consume_DeveRegistrarRejeicao_QuandoMensagemIndicarRejeicao()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerErro();
        var mensagem = new ProcessamentoDiagramaErroDtoBuilder().ComAnaliseDiagramaId(analiseDiagramaId).ComMotivo("Não é diagrama").Rejeitado().Build();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoErro(mensagem, Guid.NewGuid());

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarRejeitado();
        resultado.Erros[^1].Mensagem.Valor.ShouldBe("Não é diagrama");
    }

    [Fact(DisplayName = "Não deve alterar estado quando resultado não existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaErroConsumer")]
    public async Task Consume_NaoDeveAlterarEstado_QuandoResultadoNaoExistir()
    {
        // Arrange
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = fixture.CriarConsumerErro();
        var mensagem = new ProcessamentoDiagramaErroDtoBuilder().ComAnaliseDiagramaId(Guid.NewGuid()).ComMotivo("Imagem corrompida").Build();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoErro(mensagem);
        var quantidadeAntes = await fixture.Contexto.ResultadosDiagrama.CountAsync();

        // Act
        await consumer.Consume(contexto.Object);
        var quantidadeDepois = await fixture.Contexto.ResultadosDiagrama.CountAsync();

        // Assert
        quantidadeAntes.ShouldBe(0);
        quantidadeDepois.ShouldBe(quantidadeAntes);
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto falha")]
    [Trait("Infrastructure", "ProcessamentoDiagramaErroConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoFalha()
    {
        // Arrange
        var consumer = ResultadoDiagramaConsumerTestFixture.CriarConsumerErroComContextoDescartado();
        var mensagem = new ProcessamentoDiagramaErroDtoBuilder().Build();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoErro(mensagem);

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}
