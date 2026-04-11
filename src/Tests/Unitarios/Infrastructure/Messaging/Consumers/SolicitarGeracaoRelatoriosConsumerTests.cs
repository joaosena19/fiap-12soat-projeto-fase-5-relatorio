using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Relatorios;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers.Extensions;
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
        strategyMock.ComTipoRelatorio(TipoRelatorioEnum.Json);
        strategyMock.AoGerar().Retorna(Conteudos.Criar(new Dictionary<string, string> { ["jsonString"] = "{}" }));

        var resolverMock = new Mock<IRelatorioStrategyResolver>();
        resolverMock.AoResolver(TipoRelatorioEnum.Json).Retorna(strategyMock.Object);

        var consumer = fixture.CriarConsumerSolicitacao(resolverMock.Object);
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoSolicitacao(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = [TipoRelatorioEnum.Json]
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.ObterRelatorio(TipoRelatorioEnum.Json).Status.Valor.ShouldBe(StatusRelatorioEnum.Concluido);
        strategyMock.DeveTerGerado();
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
        strategyMock.ComTipoRelatorio(TipoRelatorioEnum.Markdown);
        strategyMock.AoGerar().Retorna(Conteudos.Criar(new Dictionary<string, string> { ["conteudo"] = "# Relatório" }));

        var resolverMock = new Mock<IRelatorioStrategyResolver>();
        resolverMock.AoResolver(TipoRelatorioEnum.Markdown).Retorna(strategyMock.Object);

        var consumer = fixture.CriarConsumerSolicitacao(resolverMock.Object);
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoSolicitacao(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = [TipoRelatorioEnum.Markdown, TipoRelatorioEnum.Markdown]
        });

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        strategyMock.DeveTerGerado();
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

        var consumer = fixture.CriarConsumerSolicitacao(resolverMock.Object);
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoSolicitacao(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            TiposRelatorio = [TipoRelatorioEnum.Json]
        });

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.DeveTerErroDoTipoRelatorio(TipoRelatorioEnum.Json);
    }

    [Fact(DisplayName = "Deve relançar exceção quando contexto de banco falha")]
    [Trait("Infrastructure", "SolicitarGeracaoRelatoriosConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoContextoBancoFalha()
    {
        // Arrange
        var resolverMock = new Mock<IRelatorioStrategyResolver>();
        var consumer = ResultadoDiagramaConsumerTestFixture.CriarConsumerSolicitacaoComContextoDescartado(resolverMock.Object);
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoSolicitacao(new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            TiposRelatorio = [TipoRelatorioEnum.Json]
        });

        // Act & Assert
        await Should.ThrowAsync<Exception>(() => consumer.Consume(contexto.Object));
    }
}
