using Application.Contracts.Messaging.Dtos;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class UploadDiagramaConcluidoConsumerTests
{
    [Fact(DisplayName = "Deve criar resultado quando upload concluído não existir")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveCriarResultado_QuandoNaoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = fixture.CriarConsumerConcluido();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoConcluido(new UploadDiagramaConcluidoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            NomeOriginal = "diagrama.png",
            Extensao = ".png"
        });

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        fixture.Contexto.ResultadosDiagrama.Any(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBeTrue();
    }

    [Fact(DisplayName = "Não deve duplicar resultado quando upload concluído já existir")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_NaoDeveDuplicarResultado_QuandoJaExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Build());
        var consumer = fixture.CriarConsumerConcluido();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoConcluido(new UploadDiagramaConcluidoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            NomeOriginal = "diagrama.png",
            Extensao = ".png"
        });

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto falha")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoFalha()
    {
        // Arrange
        var consumer = ResultadoDiagramaConsumerTestFixture.CriarConsumerConcluidoComContextoDescartado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoConcluido(new UploadDiagramaConcluidoDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            NomeOriginal = "diagrama.png"
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }

    [Fact(DisplayName = "Deve transicionar para EmProcessamento quando status for Erro (retry)")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveTransicionarParaEmProcessamento_QuandoStatusErro()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).ComFalhaProcessamento().Build());
        var consumer = fixture.CriarConsumerConcluido();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoConcluido(new UploadDiagramaConcluidoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            NomeOriginal = "diagrama.png",
            Extensao = ".png"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarComStatus(StatusAnaliseEnum.EmProcessamento);
        resultado.AnaliseResultado.ShouldBeNull();
        resultado.Relatorios.Count.ShouldBe(Enum.GetValues<TipoRelatorioEnum>().Length);
        resultado.Erros.ShouldNotBeEmpty();
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve ignorar quando status for Analisado")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveIgnorar_QuandoStatusAnalisado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().Build());
        var consumer = fixture.CriarConsumerConcluido();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoConcluido(new UploadDiagramaConcluidoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            NomeOriginal = "diagrama.png",
            Extensao = ".png"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarComStatus(StatusAnaliseEnum.Analisado);
        resultado.AnaliseResultado.ShouldNotBeNull();
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve ignorar quando status for EmProcessamento")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveIgnorar_QuandoStatusEmProcessamento()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerConcluido();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoConcluido(new UploadDiagramaConcluidoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            NomeOriginal = "diagrama.png",
            Extensao = ".png"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarComStatus(StatusAnaliseEnum.EmProcessamento);
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve ignorar quando status for Rejeitado")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveIgnorar_QuandoStatusRejeitado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Rejeitado().Build());
        var consumer = fixture.CriarConsumerConcluido();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoConcluido(new UploadDiagramaConcluidoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            NomeOriginal = "diagrama.png",
            Extensao = ".png"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarComStatus(StatusAnaliseEnum.Rejeitado);
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }
}
