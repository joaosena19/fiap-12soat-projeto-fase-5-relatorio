using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Relatorios;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using Infrastructure.Database;
using Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class SolicitarGeracaoRelatoriosConsumerTests
{
    [Fact(DisplayName = "Deve gerar relatório chamando strategy quando resultado analisado existir")]
    [Trait("Infrastructure", "SolicitarGeracaoRelatoriosConsumer")]
    public async Task Consume_DeveGerarRelatorio_QuandoResultadoAnalisadoExistir()
    {
        // Arrange
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
        contexto.SetupGet(item => item.Message).Returns(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = [TipoRelatorioEnum.Json]
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.ObterRelatorio(TipoRelatorioEnum.Json).Status.Valor.ShouldBe(StatusRelatorioEnum.Concluido);
        strategyMock.Verify(item => item.GerarAsync(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>()), Times.Once);
    }

    [Fact(DisplayName = "Deve gerar cada tipo apenas uma vez quando tipos duplicados forem enviados")]
    [Trait("Infrastructure", "SolicitarGeracaoRelatoriosConsumer")]
    public async Task Consume_DeveGerarUmaVezPorTipo_QuandoTiposDuplicados()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().Build());

        var strategyMock = new Mock<IRelatorioStrategy>();
        strategyMock.SetupGet(item => item.TipoRelatorio).Returns(TipoRelatorioEnum.Markdown);
        strategyMock.AoGerar().Retorna(Conteudos.Criar(new Dictionary<string, string> { ["conteudo"] = "# Relatório" }));

        var resolverMock = new Mock<IRelatorioStrategyResolver>();
        resolverMock.AoResolver(TipoRelatorioEnum.Markdown).Retorna(strategyMock.Object);

        var consumer = new SolicitarGeracaoRelatoriosConsumer(fixture.Contexto, resolverMock.Object, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<SolicitarGeracaoRelatoriosDto>>();
        contexto.SetupGet(item => item.Message).Returns(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = [TipoRelatorioEnum.Markdown, TipoRelatorioEnum.Markdown]
        });

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        strategyMock.Verify(item => item.GerarAsync(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>()), Times.Once);
    }

    [Fact(DisplayName = "Deve registrar falha no relatório quando strategy lança exceção")]
    [Trait("Infrastructure", "SolicitarGeracaoRelatoriosConsumer")]
    public async Task Consume_DeveRegistrarFalhaRelatorio_QuandoStrategyLancaExcecao()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().Build());

        var strategyMock = new Mock<IRelatorioStrategy>();
        strategyMock.AoGerar().LancaExcecao(new InvalidOperationException("Falha na geração do relatório"));

        var resolverMock = new Mock<IRelatorioStrategyResolver>();
        resolverMock.AoResolver(TipoRelatorioEnum.Json).Retorna(strategyMock.Object);

        var consumer = new SolicitarGeracaoRelatoriosConsumer(fixture.Contexto, resolverMock.Object, fixture.FabricaLogger);
        var contexto = new Mock<ConsumeContext<SolicitarGeracaoRelatoriosDto>>();
        contexto.SetupGet(item => item.Message).Returns(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = [TipoRelatorioEnum.Json]
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.Erros.ShouldNotBeEmpty();
        resultado.Erros.Any(item => item.TipoRelatorio.Valor == TipoRelatorioEnum.Json).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto de banco falha")]
    [Trait("Infrastructure", "SolicitarGeracaoRelatoriosConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoBancoFalha()
    {
        // Arrange
        using var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-solicitar-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var resolverMock = new Mock<IRelatorioStrategyResolver>();
        var consumer = new SolicitarGeracaoRelatoriosConsumer(contextoDb, resolverMock.Object, loggerFactory);
        contextoDb.Dispose();

        var contexto = new Mock<ConsumeContext<SolicitarGeracaoRelatoriosDto>>();
        contexto.SetupGet(item => item.Message).Returns(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            TiposRelatorio = [TipoRelatorioEnum.Json]
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}
