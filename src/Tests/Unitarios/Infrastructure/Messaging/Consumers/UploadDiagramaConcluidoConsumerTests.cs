using Application.Contracts.Messaging.Dtos;
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
}
