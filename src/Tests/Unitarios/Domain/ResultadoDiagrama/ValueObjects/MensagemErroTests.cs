using MensagemErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.Mensagem;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class MensagemErroTests
{
    [Fact(DisplayName = "Deve criar mensagem de erro quando valor é válido")]
    [Trait("ValueObject", "MensagemErro")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var mensagemErro = new MensagemErro(" Falha ao processar ");

        // Assert
        mensagemErro.Valor.ShouldBe("Falha ao processar");
    }

    [Fact(DisplayName = "Deve lançar exceção quando mensagem de erro é vazia")]
    [Trait("ValueObject", "MensagemErro")]
    public void Constructor_DeveLancarExcecao_QuandoValorVazio()
    {
        // Arrange
        Action acao = () => _ = new MensagemErro(" ");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Mensagem de erro");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "MensagemErro")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (MensagemErro)Activator.CreateInstance(typeof(MensagemErro), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}