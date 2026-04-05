using Application.Contracts.Messaging.Dtos;
using Infrastructure.Database;
using Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class UploadDiagramaRejeitadoConsumerTests
{
    [Fact(DisplayName = "Deve criar e registrar falha quando resultado não existir")]
    [Trait("Infrastructure", "UploadDiagramaRejeitadoConsumer")]
    public async Task Consume_DeveCriarERegistrarFalha_QuandoResultadoNaoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = new UploadDiagramaRejeitadoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<UploadDiagramaRejeitadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new UploadDiagramaRejeitadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            MotivoRejeicao = "Malware detectado"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.ShouldNotBeNull();
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Erro);
        resultado.Erros[^1].Mensagem.Valor.ShouldBe("Malware detectado");
    }

    [Fact(DisplayName = "Deve atualizar resultado existente com falha ao rejeitar upload")]
    [Trait("Infrastructure", "UploadDiagramaRejeitadoConsumer")]
    public async Task Consume_DeveAtualizarResultadoExistente_ComFalha()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = new UploadDiagramaRejeitadoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<UploadDiagramaRejeitadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new UploadDiagramaRejeitadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            MotivoRejeicao = "Formato inválido"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Erro);
        resultado.Erros.ShouldNotBeEmpty();
        resultado.Erros[^1].Mensagem.Valor.ShouldBe("Formato inválido");
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto falha")]
    [Trait("Infrastructure", "UploadDiagramaRejeitadoConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoFalha()
    {
        // Arrange
        using var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-rejeitado-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new UploadDiagramaRejeitadoConsumer(contextoDb, loggerFactory);
        contextoDb.Dispose();

        var contexto = new Mock<ConsumeContext<UploadDiagramaRejeitadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new UploadDiagramaRejeitadoDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            MotivoRejeicao = "Arquivo inválido"
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}
