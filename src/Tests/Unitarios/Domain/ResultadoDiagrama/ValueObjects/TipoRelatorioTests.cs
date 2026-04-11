using TipoRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Tipo;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class TipoRelatorioTests
{
    [Theory(DisplayName = "Deve criar tipo de relatório com valor válido")]
    [InlineData(TipoRelatorioEnum.Json)]
    [InlineData(TipoRelatorioEnum.Markdown)]
    [InlineData(TipoRelatorioEnum.Pdf)]
    [Trait("ValueObject", "TipoRelatorio")]
    public void Constructor_DeveCriar_QuandoValorValido(TipoRelatorioEnum valor)
    {
        // Act
        var tipoRelatorio = new TipoRelatorio(valor);

        // Assert
        tipoRelatorio.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando tipo de relatório é inválido")]
    [Trait("ValueObject", "TipoRelatorio")]
    public void Constructor_DeveLancarExcecao_QuandoValorInvalido()
    {
        // Arrange
        Action acao = () => _ = new TipoRelatorio((TipoRelatorioEnum)999);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Tipo de relatório");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "TipoRelatorio")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (TipoRelatorio)Activator.CreateInstance(typeof(TipoRelatorio), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(default(TipoRelatorioEnum));
    }
}