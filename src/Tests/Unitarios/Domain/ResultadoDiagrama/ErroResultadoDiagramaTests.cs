using Domain.ResultadoDiagrama.Aggregates;

namespace Tests.Domain.ResultadoDiagrama;

public class ErroResultadoDiagramaTests
{
    [Fact(DisplayName = "Deve criar erro quando dados são válidos")]
    [Trait("Entity", "ErroResultadoDiagrama")]
    public void Criar_DeveCriar_QuandoDadosValidos()
    {
        // Act
        var erroResultadoDiagrama = ErroResultadoDiagrama.Criar("Falha ao gerar relatório", TipoRelatorioEnum.Markdown);

        // Assert
        erroResultadoDiagrama.DeveConterDados("Falha ao gerar relatório", TipoRelatorioEnum.Markdown);
    }

    [Fact(DisplayName = "Deve reidratar erro preservando valores")]
    [Trait("Entity", "ErroResultadoDiagrama")]
    public void Reidratar_DevePreservarValores_QuandoChamado()
    {
        // Arrange
        var dataOcorrencia = DateTimeOffset.UtcNow;

        // Act
        var erroResultadoDiagrama = ErroResultadoDiagrama.Reidratar("Erro externo", null, null, null, dataOcorrencia);

        // Assert
        erroResultadoDiagrama.DeveConterDados("Erro externo", null, dataOcorrencia);
    }

    [Fact(DisplayName = "Deve criar erro com origem e número de tentativa")]
    [Trait("Entity", "ErroResultadoDiagrama")]
    public void Criar_DeveCriarComOrigemETentativa_QuandoInformados()
    {
        // Act
        var erroResultadoDiagrama = ErroResultadoDiagrama.Criar("Falha LLM", null, OrigemErroEnum.Llm, 2);

        // Assert
        erroResultadoDiagrama.Mensagem.Valor.ShouldBe("Falha LLM");
        erroResultadoDiagrama.OrigemErro.Valor.ShouldBe(OrigemErroEnum.Llm);
        erroResultadoDiagrama.NumeroTentativa.Valor.ShouldBe(2);
    }

    [Fact(DisplayName = "Deve reidratar erro com origem e número de tentativa")]
    [Trait("Entity", "ErroResultadoDiagrama")]
    public void Reidratar_DevePreservarOrigemETentativa_QuandoInformados()
    {
        // Arrange
        var dataOcorrencia = DateTimeOffset.UtcNow;

        // Act
        var erroResultadoDiagrama = ErroResultadoDiagrama.Reidratar("Erro processamento", TipoRelatorioEnum.Json, OrigemErroEnum.Processamento, 1, dataOcorrencia);

        // Assert
        erroResultadoDiagrama.OrigemErro.Valor.ShouldBe(OrigemErroEnum.Processamento);
        erroResultadoDiagrama.NumeroTentativa.Valor.ShouldBe(1);
        erroResultadoDiagrama.DeveConterDados("Erro processamento", TipoRelatorioEnum.Json, dataOcorrencia);
    }
}