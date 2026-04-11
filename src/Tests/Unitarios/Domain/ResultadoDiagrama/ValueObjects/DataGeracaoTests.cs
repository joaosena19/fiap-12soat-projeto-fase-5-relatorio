using DataGeracaoRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.DataGeracao;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class DataGeracaoTests
{
    [Fact(DisplayName = "Deve criar data de geração com valor válido")]
    [Trait("ValueObject", "DataGeracao")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = DateTimeOffset.UtcNow;

        // Act
        var dataGeracao = new DataGeracaoRelatorio(valor);

        // Assert
        dataGeracao.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando data de geração é default")]
    [Trait("ValueObject", "DataGeracao")]
    public void Constructor_DeveLancarExcecao_QuandoValorDefault()
    {
        // Arrange
        Action acao = () => _ = new DataGeracaoRelatorio(default);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Data de geração inválida");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "DataGeracao")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (DataGeracaoRelatorio)Activator.CreateInstance(typeof(DataGeracaoRelatorio), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(default(DateTimeOffset));
    }
}