using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using ResultadoDiagramaAggregate = global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama;

namespace Tests.Domain.ResultadoDiagrama;

public class ResultadoDiagramaAggregateTests
{
    [Fact(DisplayName = "Deve criar agregado com estado inicial esperado")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void Criar_DeveCriarComEstadoInicial_QuandoAnaliseDiagramaIdValido()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();

        // Act
        var resultadoDiagrama = ResultadoDiagramaAggregate.Criar(analiseDiagramaId);

        // Assert
        resultadoDiagrama.Id.ShouldNotBe(Guid.Empty);
        resultadoDiagrama.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        resultadoDiagrama.Status.Valor.ShouldBe(StatusAnaliseEnum.Recebido);
        resultadoDiagrama.AnaliseResultado.ShouldBeNull();
        resultadoDiagrama.Relatorios.Count.ShouldBe(Enum.GetValues<TipoRelatorioEnum>().Length);
        resultadoDiagrama.Relatorios.All(item => item.Status.Valor == StatusRelatorioEnum.NaoSolicitado).ShouldBeTrue();
        resultadoDiagrama.Erros.Count.ShouldBe(0);
        resultadoDiagrama.DataCriacao.Valor.ShouldNotBe(default);
    }

    [Fact(DisplayName = "Deve marcar em processamento quando status é recebido")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void MarcarEmProcessamento_DeveAlterarStatus_QuandoStatusRecebido()
    {
        // Arrange
        var resultadoDiagrama = ResultadoDiagramaAggregate.Criar(Guid.NewGuid());

        // Act
        resultadoDiagrama.MarcarEmProcessamento();

        // Assert
        resultadoDiagrama.Status.Valor.ShouldBe(StatusAnaliseEnum.EmProcessamento);
    }

    [Fact(DisplayName = "Deve lançar exceção ao marcar em processamento em estado inválido")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void MarcarEmProcessamento_DeveLancarExcecao_QuandoStatusNaoRecebido()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().EmProcessamento().Build();
        Action acao = () => resultadoDiagrama.MarcarEmProcessamento();

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Só é possível marcar EmProcessamento");
    }

    [Fact(DisplayName = "Deve registrar análise e marcar status analisado")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void RegistrarAnalise_DeveAtualizarStatusEAnalise_QuandoChamado()
    {
        // Arrange
        var resultadoDiagrama = ResultadoDiagramaAggregate.Criar(Guid.NewGuid());
        resultadoDiagrama.MarcarEmProcessamento();
        var analiseResultado = AnaliseResultado.Criar("Análise", ["API"], ["Risco"], ["Recomendação"]);

        // Act
        resultadoDiagrama.RegistrarAnalise(analiseResultado);

        // Assert
        resultadoDiagrama.Status.Valor.ShouldBe(StatusAnaliseEnum.Analisado);
        resultadoDiagrama.AnaliseResultado.ShouldNotBeNull();
        resultadoDiagrama.AnaliseDisponivel().ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve registrar falha de processamento")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void RegistrarFalhaProcessamento_DeveRegistrarErro_QuandoChamado()
    {
        // Arrange
        var resultadoDiagrama = ResultadoDiagramaAggregate.Criar(Guid.NewGuid());

        // Act
        resultadoDiagrama.RegistrarFalhaProcessamento("Falha ao processar");

        // Assert
        resultadoDiagrama.Status.Valor.ShouldBe(StatusAnaliseEnum.Erro);
        resultadoDiagrama.Erros.Count.ShouldBe(1);
        resultadoDiagrama.Erros[0].TipoRelatorio.Valor.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve retornar aceito para geração para relatório não solicitado")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void ObterResultadoSolicitacaoGeracaoRelatorio_DeveRetornarAceito_QuandoRelatorioNaoSolicitado()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();

        // Act
        var resultadoSolicitacao = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum.Markdown);

        // Assert
        resultadoSolicitacao.ShouldBe(ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao);
    }

    [Fact(DisplayName = "Deve retornar em andamento quando relatório já está solicitado")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void ObterResultadoSolicitacaoGeracaoRelatorio_DeveRetornarJaEmAndamento_QuandoRelatorioSolicitado()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().ComRelatorioSolicitado(TipoRelatorioEnum.Markdown).Build();

        // Act
        var resultadoSolicitacao = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum.Markdown);

        // Assert
        resultadoSolicitacao.ShouldBe(ResultadoSolicitacaoGeracaoRelatorioEnum.JaEmAndamento);
    }

    [Fact(DisplayName = "Deve retornar concluído quando relatório já foi concluído")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void ObterResultadoSolicitacaoGeracaoRelatorio_DeveRetornarConcluido_QuandoRelatorioConcluido()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().ComRelatorioConcluido(TipoRelatorioEnum.Markdown, Conteudos.Criar(new Dictionary<string, string> { ["conteudo"] = "ok" })).Build();

        // Act
        var resultadoSolicitacao = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum.Markdown);

        // Assert
        resultadoSolicitacao.ShouldBe(ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido);
    }
}
