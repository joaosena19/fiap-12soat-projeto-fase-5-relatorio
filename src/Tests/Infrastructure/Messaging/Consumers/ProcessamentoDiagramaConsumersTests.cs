using Application.Contracts.Messaging.Dtos;
using Domain.ResultadoDiagrama.Enums;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class ProcessamentoDiagramaConsumersTests
{
    [Fact(DisplayName = "Deve registrar análise e publicar solicitação quando resultado existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaAnalisadoConsumer")]
    public async Task Consume_DeveRegistrarAnaliseEPublicar_QuandoResultadoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerAnalisado();
        var mensagem = new ProcessamentoDiagramaAnalisadoDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            DescricaoAnalise = "Descricao consolidada",
            ComponentesIdentificados = ["API", "Banco"],
            RiscosArquiteturais = ["Acoplamento"],
            RecomendacoesBasicas = ["Separar serviços"]
        };
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoAnalisado(mensagem, Guid.NewGuid());

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Analisado);
        resultado.AnaliseResultado.ShouldNotBeNull();
        resultado.AnaliseResultado.DescricaoAnalise.Valor.ShouldBe("Descricao consolidada");
        fixture.DeveTerPublicadoSolicitacaoGeracao(analiseDiagramaId);
    }

    [Fact(DisplayName = "Não deve publicar quando resultado analisado não existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaAnalisadoConsumer")]
    public async Task Consume_NaoDevePublicar_QuandoResultadoNaoExistir()
    {
        // Arrange
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = fixture.CriarConsumerAnalisado();
        var mensagem = new ProcessamentoDiagramaAnalisadoDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            DescricaoAnalise = "Descricao consolidada",
            ComponentesIdentificados = ["API"],
            RiscosArquiteturais = ["Risco"],
            RecomendacoesBasicas = ["Recomendacao"]
        };
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoAnalisado(mensagem);

        // Act
        await consumer.Consume(contexto.Object);

        // Assert
        fixture.NaoDeveTerPublicadoSolicitacaoGeracao();
    }

    [Fact(DisplayName = "Deve registrar falha de processamento quando resultado existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaErroConsumer")]
    public async Task Consume_DeveRegistrarFalha_QuandoResultadoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        using var fixture = new ResultadoDiagramaConsumerTestFixture()
            .ComResultadoDiagrama(new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).EmProcessamento().Build());
        var consumer = fixture.CriarConsumerErro();
        var mensagem = new ProcessamentoDiagramaErroDto
        {
            AnaliseDiagramaId = analiseDiagramaId,
            Motivo = "Timeout no processamento",
            TentativasRealizadas = 3
        };
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoErro(mensagem, Guid.NewGuid());

        // Act
        await consumer.Consume(contexto.Object);
        var resultado = await fixture.Contexto.ResultadosDiagrama.FirstAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Erro);
        resultado.Erros.ShouldNotBeEmpty();
        resultado.Erros[^1].Mensagem.Valor.ShouldBe("Timeout no processamento");
    }

    [Fact(DisplayName = "Não deve lançar exceção quando resultado com erro não existir")]
    [Trait("Infrastructure", "ProcessamentoDiagramaErroConsumer")]
    public async Task Consume_NaoDeveLancarExcecao_QuandoResultadoNaoExistir()
    {
        // Arrange
        using var fixture = new ResultadoDiagramaConsumerTestFixture();
        var consumer = fixture.CriarConsumerErro();
        var mensagem = new ProcessamentoDiagramaErroDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            Motivo = "Timeout no processamento",
            TentativasRealizadas = 2
        };
        var contexto = ResultadoDiagramaConsumerTestFixture.CriarContextoErro(mensagem);

        // Act
        var acao = () => consumer.Consume(contexto.Object);

        // Assert
        await acao.ShouldNotThrowAsync();
    }
}