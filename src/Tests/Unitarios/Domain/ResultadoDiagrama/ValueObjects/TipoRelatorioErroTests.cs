using TipoRelatorioErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.TipoRelatorio;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class TipoRelatorioErroTests
{
    [Fact(DisplayName = "Deve criar tipo de relatório de erro com valor nulo")]
    [Trait("ValueObject", "TipoRelatorioErro")]
    public void Constructor_DeveCriar_QuandoValorNulo()
    {
        // Act
        var tipoRelatorioErro = new TipoRelatorioErro(null);

        // Assert
        tipoRelatorioErro.Valor.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve criar tipo de relatório de erro com valor válido")]
    [Trait("ValueObject", "TipoRelatorioErro")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var tipoRelatorioErro = new TipoRelatorioErro(TipoRelatorioEnum.Json);

        // Assert
        tipoRelatorioErro.Valor.ShouldBe(TipoRelatorioEnum.Json);
    }

    [Fact(DisplayName = "Deve lançar exceção quando tipo de relatório de erro é inválido")]
    [Trait("ValueObject", "TipoRelatorioErro")]
    public void Constructor_DeveLancarExcecao_QuandoValorInvalido()
    {
        // Arrange
        Action acao = () => _ = new TipoRelatorioErro((TipoRelatorioEnum)999);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Tipo de relatório");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "TipoRelatorioErro")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (TipoRelatorioErro)Activator.CreateInstance(typeof(TipoRelatorioErro), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBeNull();
    }
}