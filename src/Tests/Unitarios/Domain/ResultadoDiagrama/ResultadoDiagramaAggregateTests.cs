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
        resultadoDiagrama.DeveEstarRecentementeCriado(analiseDiagramaId);
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
        resultadoDiagrama.DeveEstarComStatus(StatusAnaliseEnum.EmProcessamento);
    }

    [Fact(DisplayName = "Deve ser idempotente ao marcar em processamento quando já está em processamento")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void MarcarEmProcessamento_DeveSerIdempotente_QuandoJaEmProcessamento()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().EmProcessamento().Build();

        // Act
        resultadoDiagrama.MarcarEmProcessamento();

        // Assert
        resultadoDiagrama.DeveEstarComStatus(StatusAnaliseEnum.EmProcessamento);
    }

    [Fact(DisplayName = "Deve lançar exceção ao marcar em processamento em estado inválido")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void MarcarEmProcessamento_DeveLancarExcecao_QuandoStatusNaoRecebido()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().Build();
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
        var analiseResultado = new AnaliseResultadoBuilder().Build();

        // Act
        resultadoDiagrama.RegistrarAnalise(analiseResultado);

        // Assert
        resultadoDiagrama.DeveEstarAnalisado();
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
        resultadoDiagrama.DeveEstarComErroSemRelatorio();
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
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Analisado().ComRelatorioConcluido(TipoRelatorioEnum.Markdown, ConteudosBuilder.CriarPadrao()).Build();

        // Act
        var resultadoSolicitacao = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum.Markdown);

        // Assert
        resultadoSolicitacao.ShouldBe(ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido);
    }

    [Fact(DisplayName = "Deve retornar análise não disponível quando status é Erro")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void ObterResultadoSolicitacaoGeracaoRelatorio_DeveRetornarAnaliseNaoDisponivel_QuandoStatusErro()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComFalhaProcessamento().Build();

        // Act
        var resultadoSolicitacao = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum.Markdown);

        // Assert
        resultadoSolicitacao.ShouldBe(ResultadoSolicitacaoGeracaoRelatorioEnum.AnaliseNaoDisponivel);
    }

    [Fact(DisplayName = "Deve retornar análise não disponível quando status é Recebido")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void ObterResultadoSolicitacaoGeracaoRelatorio_DeveRetornarAnaliseNaoDisponivel_QuandoStatusRecebido()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().Build();

        // Act
        var resultadoSolicitacao = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum.Markdown);

        // Assert
        resultadoSolicitacao.ShouldBe(ResultadoSolicitacaoGeracaoRelatorioEnum.AnaliseNaoDisponivel);
    }

    [Fact(DisplayName = "Deve retornar análise não disponível quando em processamento sem análise")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void ObterResultadoSolicitacaoGeracaoRelatorio_DeveRetornarAnaliseNaoDisponivel_QuandoEmProcessamentoSemAnalise()
    {
        // Arrange
        var resultadoDiagrama = new ResultadoDiagramaBuilder().EmProcessamento().Build();

        // Act
        var resultadoSolicitacao = resultadoDiagrama.ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum.Markdown);

        // Assert
        resultadoSolicitacao.ShouldBe(ResultadoSolicitacaoGeracaoRelatorioEnum.AnaliseNaoDisponivel);
    }

    #region PrepararParaReprocessamento

    [Fact(DisplayName = "Deve preparar para reprocessamento quando status é Erro")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void PrepararParaReprocessamento_DeveResetarRelatoriosEStatus_QuandoStatusErro()
    {
        // Arrange
        var resultado = new ResultadoDiagramaBuilder().ComFalhaProcessamento().Build();

        // Act
        resultado.PrepararParaReprocessamento();

        // Assert
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.EmProcessamento);
        resultado.AnaliseResultado.ShouldBeNull();
        resultado.Relatorios.Count.ShouldBe(Enum.GetValues<TipoRelatorioEnum>().Length);
        resultado.Relatorios.First(r => r.Tipo.Valor == TipoRelatorioEnum.Json).Status.Valor.ShouldBe(StatusRelatorioEnum.Automatico);
        resultado.Relatorios.Where(r => r.Tipo.Valor != TipoRelatorioEnum.Json).All(r => r.Status.Valor == StatusRelatorioEnum.NaoSolicitado).ShouldBeTrue();
        resultado.Erros.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "Não deve preparar para reprocessamento quando status não é Erro")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void PrepararParaReprocessamento_DeveLancarExcecao_QuandoStatusNaoErro()
    {
        // Arrange
        var resultado = new ResultadoDiagramaBuilder().Build();
        Action acao = () => resultado.PrepararParaReprocessamento();

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("status atual for Erro");
    }

    [Fact(DisplayName = "Deve manter erros históricos ao preparar para reprocessamento")]
    [Trait("Aggregate", "ResultadoDiagrama")]
    public void PrepararParaReprocessamento_DeveManterHistoricoErros_QuandoStatusErro()
    {
        // Arrange
        var resultado = new ResultadoDiagramaBuilder().ComFalhaProcessamento().Build();
        var quantidadeErrosAntes = resultado.Erros.Count;

        // Act
        resultado.PrepararParaReprocessamento();

        // Assert
        resultado.Erros.Count.ShouldBe(quantidadeErrosAntes);
    }

    #endregion
}
