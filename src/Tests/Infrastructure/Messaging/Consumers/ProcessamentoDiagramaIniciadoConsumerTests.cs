using Application.Contracts.Messaging.Dtos;
using Infrastructure.Database;
using Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        var consumer = new ProcessamentoDiagramaIniciadoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<ProcessamentoDiagramaIniciadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new ProcessamentoDiagramaIniciadoDto
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
        var consumer = new ProcessamentoDiagramaIniciadoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<ProcessamentoDiagramaIniciadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new ProcessamentoDiagramaIniciadoDto
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
        using var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-iniciado-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new ProcessamentoDiagramaIniciadoConsumer(contextoDb, loggerFactory);
        contextoDb.Dispose();

        var contexto = new Mock<ConsumeContext<ProcessamentoDiagramaIniciadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new ProcessamentoDiagramaIniciadoDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            Extensao = ".png"
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}
