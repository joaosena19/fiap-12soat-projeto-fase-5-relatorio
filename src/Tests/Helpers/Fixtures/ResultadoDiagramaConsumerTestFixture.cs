using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Relatorios;
using Infrastructure.Database;
using Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tests.Helpers.Fixtures;

public class ResultadoDiagramaConsumerTestFixture : IDisposable
{
    public AppDbContext Contexto { get; }
    public Mock<IRelatorioMessagePublisher> RelatorioMessagePublisherMock { get; } = new();
    public ILoggerFactory FabricaLogger { get; }

    public ResultadoDiagramaConsumerTestFixture()
    {
        FabricaLogger = LoggerFactory.Create(builder => { });

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"relatorio-consumer-{Guid.NewGuid()}")
            .Options;

        Contexto = new AppDbContext(options);
    }

    public ResultadoDiagramaConsumerTestFixture ComResultadoDiagrama(global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama)
    {
        Contexto.ResultadosDiagrama.Add(resultadoDiagrama);
        Contexto.SaveChanges();
        return this;
    }

    public ProcessamentoDiagramaAnalisadoConsumer CriarConsumerAnalisado()
    {
        return new ProcessamentoDiagramaAnalisadoConsumer(Contexto, RelatorioMessagePublisherMock.Object, FabricaLogger);
    }

    public ProcessamentoDiagramaErroConsumer CriarConsumerErro()
    {
        return new ProcessamentoDiagramaErroConsumer(Contexto, FabricaLogger);
    }

    public UploadDiagramaConcluidoConsumer CriarConsumerConcluido() => new(Contexto, FabricaLogger);

    public UploadDiagramaRejeitadoConsumer CriarConsumerRejeitado() => new(Contexto, FabricaLogger);

    public ProcessamentoDiagramaIniciadoConsumer CriarConsumerIniciado() => new(Contexto, FabricaLogger);

    public SolicitarGeracaoRelatoriosConsumer CriarConsumerSolicitacao(IRelatorioStrategyResolver resolver) => new(Contexto, resolver, FabricaLogger);

    public static ProcessamentoDiagramaAnalisadoConsumer CriarConsumerAnalisadoComContextoDescartado()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-analisado-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new ProcessamentoDiagramaAnalisadoConsumer(contextoDb, new Mock<IRelatorioMessagePublisher>().Object, loggerFactory);
        contextoDb.Dispose();
        return consumer;
    }

    public static ProcessamentoDiagramaErroConsumer CriarConsumerErroComContextoDescartado()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-erro-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new ProcessamentoDiagramaErroConsumer(contextoDb, loggerFactory);
        contextoDb.Dispose();
        return consumer;
    }

    public static UploadDiagramaConcluidoConsumer CriarConsumerConcluidoComContextoDescartado()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-concluido-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new UploadDiagramaConcluidoConsumer(contextoDb, loggerFactory);
        contextoDb.Dispose();
        return consumer;
    }

    public static UploadDiagramaRejeitadoConsumer CriarConsumerRejeitadoComContextoDescartado()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-rejeitado-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new UploadDiagramaRejeitadoConsumer(contextoDb, loggerFactory);
        contextoDb.Dispose();
        return consumer;
    }

    public static ProcessamentoDiagramaIniciadoConsumer CriarConsumerIniciadoComContextoDescartado()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-iniciado-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new ProcessamentoDiagramaIniciadoConsumer(contextoDb, loggerFactory);
        contextoDb.Dispose();
        return consumer;
    }

    public static SolicitarGeracaoRelatoriosConsumer CriarConsumerSolicitacaoComContextoDescartado(IRelatorioStrategyResolver resolver)
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"test-excecao-solicitar-{Guid.NewGuid()}")
            .Options;
        var contextoDb = new AppDbContext(options);
        var consumer = new SolicitarGeracaoRelatoriosConsumer(contextoDb, resolver, loggerFactory);
        contextoDb.Dispose();
        return consumer;
    }

    public static Mock<ConsumeContext<ProcessamentoDiagramaAnalisadoDto>> CriarContextoAnalisado(ProcessamentoDiagramaAnalisadoDto mensagem, Guid? messageId = null)
    {
        var contexto = new Mock<ConsumeContext<ProcessamentoDiagramaAnalisadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(mensagem);
        contexto.SetupGet(item => item.MessageId).Returns(messageId);
        return contexto;
    }

    public static Mock<ConsumeContext<ProcessamentoDiagramaErroDto>> CriarContextoErro(ProcessamentoDiagramaErroDto mensagem, Guid? messageId = null)
    {
        var contexto = new Mock<ConsumeContext<ProcessamentoDiagramaErroDto>>();
        contexto.SetupGet(item => item.Message).Returns(mensagem);
        contexto.SetupGet(item => item.MessageId).Returns(messageId);
        return contexto;
    }

    public static Mock<ConsumeContext<UploadDiagramaConcluidoDto>> CriarContextoConcluido(UploadDiagramaConcluidoDto mensagem)
    {
        var contexto = new Mock<ConsumeContext<UploadDiagramaConcluidoDto>>();
        contexto.SetupGet(item => item.Message).Returns(mensagem);
        return contexto;
    }

    public static Mock<ConsumeContext<UploadDiagramaRejeitadoDto>> CriarContextoRejeitado(UploadDiagramaRejeitadoDto mensagem)
    {
        var contexto = new Mock<ConsumeContext<UploadDiagramaRejeitadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(mensagem);
        return contexto;
    }

    public static Mock<ConsumeContext<ProcessamentoDiagramaIniciadoDto>> CriarContextoIniciado(ProcessamentoDiagramaIniciadoDto mensagem)
    {
        var contexto = new Mock<ConsumeContext<ProcessamentoDiagramaIniciadoDto>>();
        contexto.SetupGet(item => item.Message).Returns(mensagem);
        return contexto;
    }

    public static Mock<ConsumeContext<SolicitarGeracaoRelatoriosDto>> CriarContextoSolicitacao(SolicitarGeracaoRelatoriosDto mensagem)
    {
        var contexto = new Mock<ConsumeContext<SolicitarGeracaoRelatoriosDto>>();
        contexto.SetupGet(item => item.Message).Returns(mensagem);
        return contexto;
    }

    public void DeveTerPublicadoSolicitacaoGeracao(Guid analiseDiagramaId)
    {
        RelatorioMessagePublisherMock.Verify(item => item.PublicarSolicitacaoGeracaoAsync(analiseDiagramaId, It.IsAny<IReadOnlyCollection<TipoRelatorioEnum>>()), Times.Once);
    }

    public void NaoDeveTerPublicadoSolicitacaoGeracao()
    {
        RelatorioMessagePublisherMock.Verify(item => item.PublicarSolicitacaoGeracaoAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<TipoRelatorioEnum>>()), Times.Never);
    }

    public void Dispose()
    {
        Contexto.Dispose();
        FabricaLogger.Dispose();
    }
}