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
        var erroResultadoDiagrama = ErroResultadoDiagrama.Reidratar("Erro externo", null, dataOcorrencia);

        // Assert
        erroResultadoDiagrama.DeveConterDados("Erro externo", null, dataOcorrencia);
    }
}