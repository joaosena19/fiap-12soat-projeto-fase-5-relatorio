using Application.Contracts.Messaging.Dtos;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class ProcessamentoDiagramaIniciadoConsumerTests
{
    [Fact(DisplayName = "Deve criar resultado em processamento quando registro não existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaIniciadoConsumer")]
    public async Task Consume_DeveCriarResultadoEmProcessamento_QuandoRegistroNaoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = fixture.CriarConsumerIniciado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoIniciado(new ProcessamentoDiagramaIniciadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            Extensao = ".png",
            NomeOriginal = "diagrama.png"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.ShouldNotBeNull();
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.EmProcessamento);
    }

    [Fact(DisplayName = "Deve atualizar resultado existente para em processamento")]
    [Trait("Infrastructure", "ProcessamentoDiagramaIniciadoConsumer")]
    public async Task Consume_DeveAtualizarResultadoExistente_ParaEmProcessamento()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Build());
        var consumer = fixture.CriarConsumerIniciado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoIniciado(new ProcessamentoDiagramaIniciadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            Extensao = ".jpg",
            NomeOriginal = "diagrama.jpg"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.EmProcessamento);
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto falha")]
    [Trait("Infrastructure", "ProcessamentoDiagramaIniciadoConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoFalha()
    {
        // Arrange
        var consumer = ResultadoDiagramaConsumerTestFixture.CriarConsumerIniciadoComContextoDescartado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoIniciado(new ProcessamentoDiagramaIniciadoDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            Extensao = ".png"
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }

    [Fact(DisplayName = "Deve transicionar para EmProcessamento quando status for Erro (retry)")]
    [Trait("Infrastructure", "ProcessamentoDiagramaIniciadoConsumer")]
    public async Task Consume_DeveTransicionarParaEmProcessamento_QuandoStatusErro()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).ComFalhaProcessamento().Build());
        var consumer = fixture.CriarConsumerIniciado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoIniciado(new ProcessamentoDiagramaIniciadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            Extensao = ".png",
            NomeOriginal = "diagrama.png"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarComStatus(StatusAnaliseEnum.EmProcessamento);
        resultado.AnaliseResultado.ShouldBeNull();
        resultado.Erros.ShouldNotBeEmpty();
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve ser idempotente quando status já for EmProcessamento")]
    [Trait("Infrastructure", "ProcessamentoDiagramaIniciadoConsumer")]
    public async Task Consume_DeveSerIdempotente_QuandoJaEmProcessamento()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerIniciado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoIniciado(new ProcessamentoDiagramaIniciadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            Extensao = ".png",
            NomeOriginal = "diagrama.png"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarComStatus(StatusAnaliseEnum.EmProcessamento);
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve lançar exceção quando status for Analisado")]
    [Trait("Infrastructure", "ProcessamentoDiagramaIniciadoConsumer")]
    public async Task Consume_DeveLancarExcecao_QuandoStatusAnalisado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().Build());
        var consumer = fixture.CriarConsumerIniciado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoIniciado(new ProcessamentoDiagramaIniciadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            Extensao = ".png",
            NomeOriginal = "diagrama.png"
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}
