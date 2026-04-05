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
        erroResultadoDiagrama.Mensagem.Valor.ShouldBe("Falha ao gerar relatório");
        erroResultadoDiagrama.TipoRelatorio.Valor.ShouldBe(TipoRelatorioEnum.Markdown);
        erroResultadoDiagrama.DataOcorrencia.Valor.ShouldNotBe(default);
    }

    [Fact(DisplayName = "Deve reidratar erro preservando valores")]
    [Trait("Entity", "ErroResultadoDiagrama")]
    public void Reidratar_DevePreservarValores_QuandoChamado()
    {
        // Arrange
        var dataOcorrencia = DateTimeOffset.UtcNow;

        // Act
        var erroResultadoDiagrama = ErroResultadoDiagrama.Reidratar("Erro externo", null, dataOcorrencia);

        // Assert
        erroResultadoDiagrama.Mensagem.Valor.ShouldBe("Erro externo");
        erroResultadoDiagrama.TipoRelatorio.Valor.ShouldBeNull();
        erroResultadoDiagrama.DataOcorrencia.Valor.ShouldBe(dataOcorrencia);
    }
}