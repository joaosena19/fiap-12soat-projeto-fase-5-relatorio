using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Relatorios;
using Domain.ResultadoDiagrama.Enums;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class FluxoMensageriaConsumersTests
{
    [Fact(DisplayName = "Deve criar resultado em processamento quando mensagem de processamento iniciado não encontrar registro")]
    [Trait("Infrastructure", "ProcessamentoDiagramaIniciadoConsumer")]
    public async Task Consume_DeveCriarResultadoEmProcessamento_QuandoRegistroNaoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = new ProcessamentoDiagramaIniciadoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<ProcessamentoDiagramaIniciadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new ProcessamentoDiagramaIniciadoDto { AnaliseDiagramaId = analiseDiagramaId, Extensao = ".png" });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.EmProcessamento);
    }

    [Fact(DisplayName = "Deve criar resultado quando upload concluído ainda não existir")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveCriarResultado_QuandoUploadConcluidoNaoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = new UploadDiagramaConcluidoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<UploadDiagramaConcluidoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new UploadDiagramaConcluidoDto { AnaliseDiagramaId = analiseDiagramaId });

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        fixture.Contexto.ResultadosDiagrama.Any(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBeTrue();
    }

    [Fact(DisplayName = "Não deve duplicar resultado quando upload concluído já existir")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_NaoDeveDuplicarResultado_QuandoUploadConcluidoJaExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Build());
        var consumer = new UploadDiagramaConcluidoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<UploadDiagramaConcluidoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new UploadDiagramaConcluidoDto { AnaliseDiagramaId = analiseDiagramaId });

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        fixture.Contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve registrar falha quando upload rejeitado não encontrar resultado anterior")]
    [Trait("Infrastructure", "UploadDiagramaRejeitadoConsumer")]
    public async Task Consume_DeveRegistrarFalha_QuandoUploadRejeitadoNaoEncontrarResultado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = new UploadDiagramaRejeitadoConsumer(fixture.Contexto, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<UploadDiagramaRejeitadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(new UploadDiagramaRejeitadoDto { AnaliseDiagramaId = analiseDiagramaId, MotivoRejeicao = "Malware detectado" });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Erro);
        resultado.Erros[^1].Mensagem.Valor.ShouldBe("Malware detectado");
    }

    [Fact(DisplayName = "Deve gerar relatório apenas uma vez por tipo distinto")]
    [Trait("Infrastructure", "SolicitarGeracaoRelatoriosConsumer")]
    public async Task Consume_DeveGerarUmaVezPorTipo_QuandoTiposDuplicados()
    {
        // Arrange
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().Build());
        var strategyMock = new Mock<IRelatorioStrategy>();
        strategyMock.SetupGet(item => item.TipoRelatorio).Returns(TipoRelatorioEnum.Json);
        strategyMock.AoGerar().Retorna(Conteudos.Criar(new Dictionary<string, string> { ["jsonString"] = "{}" }));
        var resolverMock = new Mock<IRelatorioStrategyResolver>();
        resolverMock.AoResolver(TipoRelatorioEnum.Json).Retorna(strategyMock.Object);
        var consumer = new SolicitarGeracaoRelatoriosConsumer(fixture.Contexto, resolverMock.Object, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<SolicitarGeracaoRelatoriosDto>>();
        contexto.SetupGet(item => item.Message).Returns(new SolicitarGeracaoRelatoriosDto { AnaliseDiagramaId = analiseDiagramaId, TiposRelatorio = [TipoRelatorioEnum.Json, TipoRelatorioEnum.Json] });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.ObterRelatorio(TipoRelatorioEnum.Json).Status.Valor.ShouldBe(StatusRelatorioEnum.Concluido);
        strategyMock.Verify(item => item.GerarAsync(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>()), Times.Once);
    }
}