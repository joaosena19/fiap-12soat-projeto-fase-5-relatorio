using Application.Contracts.Messaging.Dtos;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers.Extensions;
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
        var consumer = fixture.CriarConsumerRejeitado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoRejeitado(new UploadDiagramaRejeitadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            MotivoRejeicao = "Malware detectado"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.ShouldNotBeNull();
        resultado.DeveEstarComErroEUltimoMotivo("Malware detectado");
    }

    [Fact(DisplayName = "Deve atualizar resultado existente com falha ao rejeitar upload")]
    [Trait("Infrastructure", "UploadDiagramaRejeitadoConsumer")]
    public async Task Consume_DeveAtualizarResultadoExistente_ComFalha()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerRejeitado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoRejeitado(new UploadDiagramaRejeitadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            MotivoRejeicao = "Formato inválido"
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveEstarComErroEUltimoMotivo("Formato inválido");
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto falha")]
    [Trait("Infrastructure", "UploadDiagramaRejeitadoConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoFalha()
    {
        // Arrange
        var consumer = ResultadoDiagramaConsumerTestFixture.CriarConsumerRejeitadoComContextoDescartado();
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoRejeitado(new UploadDiagramaRejeitadoDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            MotivoRejeicao = "Arquivo inválido"
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}
