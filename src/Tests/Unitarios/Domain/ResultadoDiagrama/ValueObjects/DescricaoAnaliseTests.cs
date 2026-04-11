using Domain.ResultadoDiagrama.ValueObjects.AnaliseResultado;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class DescricaoAnaliseTests
{
    [Fact(DisplayName = "Deve criar descrição quando valor é válido")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var descricaoAnalise = new DescricaoAnalise(" Descrição válida ");

        // Assert
        descricaoAnalise.Valor.ShouldBe("Descrição válida");
    }

    [Fact(DisplayName = "Deve lançar exceção quando descrição é vazia")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Constructor_DeveLancarExcecao_QuandoValorVazio()
    {
        // Arrange
        Action acao = () => _ = new DescricaoAnalise(" ");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Descrição da análise");
    }

    [Fact(DisplayName = "Não deve criar DescricaoAnalise excedendo 10000 caracteres")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Constructor_DeveLancarExcecao_QuandoExcedeComprimentoMaximo()
    {
        // Arrange
        var valorLongo = new string('a', 10001);
        Action acao = () => _ = new DescricaoAnalise(valorLongo);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("não pode exceder 10000 caracteres");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (DescricaoAnalise)Activator.CreateInstance(typeof(DescricaoAnalise), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}